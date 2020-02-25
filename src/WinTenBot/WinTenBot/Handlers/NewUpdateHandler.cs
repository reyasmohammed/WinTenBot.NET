using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers
{
    public class NewUpdateHandler : IUpdateHandler
    {
        private RequestProvider _requestProvider;

        public NewUpdateHandler()
        {
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            if (context.Update.ChannelPost != null) return;

            var message = context.Update.Message ?? context.Update.CallbackQuery.Message;
            var fromUser = message.From;

            Log.Information($"New Update: {context.Update.ToJson(true)}");

            // var actions = new List<Action>();
            var shouldAwaitTasks = new List<Task>();
            var nonAwaitTasks = new List<Task>();

            shouldAwaitTasks.Add(_requestProvider.CheckCasBanAsync(fromUser));
            shouldAwaitTasks.Add(_requestProvider.CheckUsername(message));
            
            nonAwaitTasks.Add(_requestProvider.AfkCheck(message));
            nonAwaitTasks.Add(_requestProvider.FindNotesAsync(message));
            nonAwaitTasks.Add(_requestProvider.HitActivity());

            // actions.Add(async () => await _requestProvider.AfkCheck(message));
            // actions.Add(async () => await _requestProvider.CheckCasBanAsync(message.From));
            // actions.Add(async () => await _requestProvider.CheckUsername(message));
            // actions.Add(async () => await _requestProvider.FindNotesAsync(message));
            // actions.Add(async () => await _requestProvider.HitActivity());

            if (context.Update.CallbackQuery == null)
            {
                // actions.Add(async () => await _requestProvider.CheckMessage(message));
                shouldAwaitTasks.Add(_requestProvider.CheckMessage(message));
            }

            if (!_requestProvider.IsPrivateChat())
            {
                // actions.Add(async () => await _requestProvider.EnsureChatRestriction());
                // actions.Add(async () => await _requestProvider.CheckGlobalBanAsync(message));

                shouldAwaitTasks.Add(_requestProvider.EnsureChatRestriction());
                shouldAwaitTasks.Add(_requestProvider.CheckGlobalBanAsync(message));
            }

            // Parallel.Invoke(actions.ToArray());
            await Task.WhenAll(shouldAwaitTasks);
            
#pragma warning disable 4014
            // This List Task should not await.
            Task.WhenAll(nonAwaitTasks);
#pragma warning restore 4014

            await next(context, cancellationToken);
        }
    }
}