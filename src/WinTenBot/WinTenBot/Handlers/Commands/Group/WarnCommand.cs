using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Group
{
    public class WarnCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            var msg = _telegramProvider.Message;

            if (!await _telegramProvider.IsAdminOrPrivateChat())
                return;

            if (msg.ReplyToMessage != null)
            {
                await _telegramProvider.WarnMemberAsync();
            }
            else
            {
                await _telegramProvider.SendTextAsync("Balas pengguna yang akan di Warn", replyToMsgId: msg.MessageId);
            }
        }
    }
}