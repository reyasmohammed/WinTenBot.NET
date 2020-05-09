using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers
{
    public class NewUpdateHandler : IUpdateHandler
    {
        private TelegramProvider _telegramProvider;

        public NewUpdateHandler()
        {
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            if (context.Update.ChannelPost != null) return;

            var message = context.Update.Message ??
                          context.Update.EditedMessage ?? context.Update.CallbackQuery.Message;
            var fromUser = message.From;

            Log.Information($"New Update: {context.Update.ToJson(true)}");

            await EnqueuePreTask();

            await next(context, cancellationToken);
        }

        private async Task EnqueuePreTask()
        {
            Log.Information("Enqueue pre tasks");

            var message = _telegramProvider.Message;
            var callbackQuery = _telegramProvider.CallbackQuery;

            // var actions = new List<Action>();
            var shouldAwaitTasks = new List<Task>();
            var nonAwaitTasks = new List<Task>();

            // if (_telegramProvider.IsNeedRunTasks())
            // {
            if (!_telegramProvider.IsPrivateChat())
            {
                shouldAwaitTasks.Add(_telegramProvider.EnsureChatRestrictionAsync());

                if (message.Text != null)
                {
                    shouldAwaitTasks.Add(_telegramProvider.CheckGlobalBanAsync());
                    shouldAwaitTasks.Add(_telegramProvider.CheckCasBanAsync());
                    shouldAwaitTasks.Add(_telegramProvider.CheckSpamWatchAsync());
                    shouldAwaitTasks.Add(_telegramProvider.CheckUsernameAsync());
                }
            }

            nonAwaitTasks.Add(_telegramProvider.EnsureChatHealthAsync());
            nonAwaitTasks.Add(_telegramProvider.AfkCheckAsync());

            if (message.Text != null)
            {
                nonAwaitTasks.Add(_telegramProvider.FindNotesAsync());
                nonAwaitTasks.Add(_telegramProvider.FindTagsAsync());
            }
            // }
            // else
            // {
            // Log.Information("Seem not need queue some Tasks..");
            // }


            nonAwaitTasks.Add(_telegramProvider.CheckMataZiziAsync());
            nonAwaitTasks.Add(_telegramProvider.HitActivityAsync());

            if (callbackQuery == null)
            {
                shouldAwaitTasks.Add(_telegramProvider.CheckMessageAsync());
            }

            // if (!_telegramProvider.IsPrivateChat())
            // {
            //     
            // }

            Log.Information($"Running {shouldAwaitTasks.Count} awaited and {nonAwaitTasks.Count} non-await tasks");

            await Task.WhenAll(shouldAwaitTasks);

#pragma warning disable 4014
            // This List Task should not await.
            Task.WhenAll(nonAwaitTasks);
#pragma warning restore 4014
        }
    }
}