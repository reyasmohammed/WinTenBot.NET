using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers.Commands.Core
{
    public class DebugCommand:CommandBase
    {
        private ChatProcessor _chatProcessor;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);

            var msg = context.Update.Message;
            var json = msg.ToJson(true);
            
            ConsoleHelper.WriteLine(json.Length);

            var sendText = $"Debug:\n {json}";
            await _chatProcessor.SendAsync(sendText);
        }
    }
}