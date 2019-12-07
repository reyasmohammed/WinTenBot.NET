using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers.Commands.Welcome
{
    public class SetWelcomeCommand:CommandBase
    {
        private ChatProcessor _chatProcessor;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            

//            var sendText = "Saved!";
            await _chatProcessor.SendAsync("Saving..");
            Thread.Sleep(1000);
            await _chatProcessor.EditAsync("Saved!");
        }
    }
}