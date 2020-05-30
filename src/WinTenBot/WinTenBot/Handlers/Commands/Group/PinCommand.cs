using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Group
{
    public class PinCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            var msg = _telegramService.MessageOrEdited;
            var client = context.Bot.Client;

            var sendText = "Balas pesan yang akan di pin";

            if (msg.ReplyToMessage != null)
            {
                var pin = client.PinChatMessageAsync(
                    msg.Chat.Id,
                    msg.ReplyToMessage.MessageId);
                return;
//                ConsoleHelper.WriteLine(pin.);
            }

            await _telegramService.SendTextAsync(sendText);
        }
    }
}