using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Services;
using WinTenBot.Telegram;

namespace WinTenBot.Handlers.Commands.Group
{
    public class WarnCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            var msg = _telegramService.Message;

            if (!await _telegramService.IsAdminOrPrivateChat()
                .ConfigureAwait(false))
                return;

            if (msg.ReplyToMessage != null)
            {
                await _telegramService.WarnMemberAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                await _telegramService.SendTextAsync("Balas pengguna yang akan di Warn", replyToMsgId: msg.MessageId)
                    .ConfigureAwait(false);
            }
        }
    }
}