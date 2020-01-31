using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    public class DebugCommand:CommandBase
    {
        private RequestProvider _requestProvider;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);

            var msg = context.Update.Message;
            var json = msg.ToJson(true);
            
            ConsoleHelper.WriteLine(json.Length);

            var sendText = $"Debug:\n {json}";
            await _requestProvider.SendTextAsync(sendText);
        }
    }
}