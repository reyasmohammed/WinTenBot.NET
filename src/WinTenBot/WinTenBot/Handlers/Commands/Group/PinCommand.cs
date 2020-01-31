using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers.Processors;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Group
{
    public class PinCommand : CommandBase
    {
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;
            var client = context.Bot.Client;

            var sendText = "Balas pesan yang akan di pin";

            if (msg.ReplyToMessage != null)
            {
                var pin = client.PinChatMessageAsync(
                    msg.Chat.Id,
                    msg.ReplyToMessage.MessageId);
//                ConsoleHelper.WriteLine(pin.);
            }

            await _requestProvider.SendTextAsync(sendText);
        }
    }
}