using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Services;
using WinTenBot.Telegram;
using WinTenBot.Text;

namespace WinTenBot.Handlers
{
    public class NewUpdateHandler : IUpdateHandler
    {
        private TelegramService _telegramService;

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            if (context.Update.ChannelPost != null) return;

            var message = context.Update.Message ??
                          context.Update.EditedMessage ?? context.Update.CallbackQuery.Message;
            var fromUser = message.From;

            if(BotSettings.IsDevelopment)
                Log.Information($"New Update: {context.Update.ToJson(true)}");

            await EnqueuePreTask();

            await next(context, cancellationToken);
            
            EnqueueBackgroundTask();
        }

        private async Task EnqueuePreTask()
        {
            Log.Information("Enqueue pre tasks");

            var message = _telegramService.Message;
            var callbackQuery = _telegramService.CallbackQuery;

            // var actions = new List<Action>();
            var shouldAwaitTasks = new List<Task>();

            // if (_telegramProvider.IsNeedRunTasks())
            // {
            if (!_telegramService.IsPrivateChat())
            {
                shouldAwaitTasks.Add(_telegramService.EnsureChatRestrictionAsync());

                if (message.Text != null)
                {
                    shouldAwaitTasks.Add(_telegramService.CheckGlobalBanAsync());
                    shouldAwaitTasks.Add(_telegramService.CheckCasBanAsync());
                    shouldAwaitTasks.Add(_telegramService.CheckSpamWatchAsync());
                    shouldAwaitTasks.Add(_telegramService.CheckUsernameAsync());
                }
            }

            if (callbackQuery == null)
            {
                shouldAwaitTasks.Add(_telegramService.CheckMessageAsync());
            }

            Log.Information("Awaiting should await task..");

            await Task.WhenAll(shouldAwaitTasks.ToArray());
        }

        private void EnqueueBackgroundTask()
        {
            var nonAwaitTasks = new List<Task>();
            var message = _telegramService.Message;

            //Exec nonAwait Tasks
            Log.Information("Running nonAwait task..");
            nonAwaitTasks.Add(_telegramService.EnsureChatHealthAsync());
            nonAwaitTasks.Add(_telegramService.AfkCheckAsync());

            if (message.Text != null)
            {
                nonAwaitTasks.Add(_telegramService.FindNotesAsync());
                nonAwaitTasks.Add(_telegramService.FindTagsAsync());
            }
            // }
            // else
            // {
            // Log.Information("Seem not need queue some Tasks..");
            // }


            nonAwaitTasks.Add(_telegramService.CheckMataZiziAsync());
            nonAwaitTasks.Add(_telegramService.HitActivityAsync());


            // if (!_telegramProvider.IsPrivateChat())
            // {
            //     
            // }


            #pragma warning disable 4014
            // This List Task should not await.
            Task.WhenAny(nonAwaitTasks.ToArray());
            #pragma warning restore 4014
            
        }
    }
}