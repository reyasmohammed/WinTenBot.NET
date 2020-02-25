using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    public class OutCommand:CommandBase
    {
        private RequestProvider _requestProvider;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        { 
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;
            var partsMsg = msg.Text.GetTextWithoutCmd().Split("|").ToArray();

            var isSudoer = _requestProvider.IsSudoer();
            if (isSudoer)
            {
                var sendText = "Maaf, saya harus keluar";

                if (partsMsg[1] != null)
                {
                    sendText += $"\n{partsMsg[1]}";
                }
                var chatId = partsMsg[0].ToInt64();

                ConsoleHelper.WriteLine($"Target out: {chatId}");
                await _requestProvider.SendTextAsync(sendText, customChatId: chatId);
                await _requestProvider.LeaveChat(chatId);
            }
            else
            {
                await _requestProvider.SendTextAsync("Kamu tidak punya hak akses.");
            }
        }
    }
}