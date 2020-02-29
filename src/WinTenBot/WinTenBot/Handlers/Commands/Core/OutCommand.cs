using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    public class OutCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            var msg = context.Update.Message;
            var partsMsg = msg.Text.GetTextWithoutCmd().Split("|").ToArray();

            var isSudoer = _telegramProvider.IsSudoer();
            if (isSudoer)
            {
                var sendText = "Maaf, saya harus keluar";

                if (partsMsg[1] != null)
                {
                    sendText += $"\n{partsMsg[1]}";
                }

                var chatId = partsMsg[0].ToInt64();
                Log.Information($"Target out: {chatId}");
                await _telegramProvider.SendTextAsync(sendText, customChatId: chatId);
                await _telegramProvider.LeaveChat(chatId);
            }
            else
            {
                await _telegramProvider.SendTextAsync("Kamu tidak punya hak akses.");
            }
        }
    }
}