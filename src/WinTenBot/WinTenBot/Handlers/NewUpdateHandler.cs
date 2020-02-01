using System;
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
        private RequestProvider _requestProvider;

        public NewUpdateHandler()
        {
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            if (context.Update.ChannelPost != null) return;

            var message = context.Update.Message ?? context.Update.CallbackQuery.Message;

            Log.Information("New Update");

            var actions = new List<Action>();

            actions.Add(async () => await _requestProvider.AfkCheck(message));
            actions.Add(async () => await _requestProvider.CheckCasBanAsync(message.From));
            actions.Add(async () => await _requestProvider.CheckUsername(message));
            actions.Add(async () => await _requestProvider.FindNotesAsync(message));
            actions.Add(() => ActivityHelper.HitActivityBackground(message));

            if (context.Update.CallbackQuery == null)
            {
                actions.Add(async () => await _requestProvider.CheckMessage(message));
            }

            if (!_requestProvider.IsPrivateChat())
            {
                actions.Add(async () => await _requestProvider.EnsureChatRestriction());
                actions.Add(async () => await _requestProvider.CheckGlobalBanAsync(message));
            }

            Parallel.Invoke(actions.ToArray());

            await next(context, cancellationToken);
        }
    }
}