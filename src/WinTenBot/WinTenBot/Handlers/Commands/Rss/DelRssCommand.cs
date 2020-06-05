using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Common;
using WinTenBot.Services;
using WinTenBot.Telegram;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class DelRssCommand : CommandBase
    {
        private RssService _rssService;
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            _rssService = new RssService(context.Update.Message);

            var isAdminOrPrivateChat = await _telegramService.IsAdminOrPrivateChat();
            if (isAdminOrPrivateChat)
            {
                var urlFeed = _telegramService.Message.Text.GetTextWithoutCmd();

                await _telegramService.SendTextAsync($"Sedang menghapus {urlFeed}");

                var delete = await _rssService.DeleteRssAsync(urlFeed);

                var success = delete.ToBool()
                    ? "berhasil."
                    : "gagal. Mungkin RSS tersebut sudah di hapus atau belum di tambahkan";

                await _telegramService.EditAsync($"Hapus {urlFeed} {success}");
            }
        }
    }
}