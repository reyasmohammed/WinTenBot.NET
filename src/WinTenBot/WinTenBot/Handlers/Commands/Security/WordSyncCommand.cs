using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Security
{
    public class WordSyncCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);

            var isSudoer = _telegramProvider.IsSudoer();
            var isAdmin = await _telegramProvider.IsAdminGroup();

            if (isSudoer)
            {
                await _telegramProvider.DeleteAsync(_telegramProvider.Message.MessageId);

                await _telegramProvider.AppendTextAsync("Sedang mengsinkronkan Word Filter");
                await DataHelper.SyncWordToLocalAsync();
                await _telegramProvider.AppendTextAsync("Selesai mengsinkronkan.");

                await _telegramProvider.DeleteAsync(delay: 3000);
            }
        }
    }
}