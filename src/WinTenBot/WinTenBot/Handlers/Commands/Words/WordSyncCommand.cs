using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Services;
using WinTenBot.Telegram;
using WinTenBot.Tools;

namespace WinTenBot.Handlers.Commands.Words
{
    public class WordSyncCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);

            var isSudoer = _telegramService.IsSudoer();
            var isAdmin = await _telegramService.IsAdminGroup();

            if (isSudoer)
            {
                await _telegramService.DeleteAsync(_telegramService.Message.MessageId);

                await _telegramService.AppendTextAsync("Sedang mengsinkronkan Word Filter");
                await Sync.SyncWordToLocalAsync();
                await _telegramService.AppendTextAsync("Selesai mengsinkronkan.");

                await _telegramService.DeleteAsync(delay: 3000);
            }
        }
    }
}