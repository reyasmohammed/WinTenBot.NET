using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class DelRssCommand : CommandBase
    {
        private RssService _rssService;
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            _rssService = new RssService(context.Update.Message);

            var isAdminOrPrivateChat = await _requestProvider.IsAdminOrPrivateChat();
            if (isAdminOrPrivateChat)
            {
                var urlFeed = _requestProvider.Message.Text.GetTextWithoutCmd();

                await _requestProvider.SendTextAsync($"Sedang menghapus {urlFeed}");

                var delete = await _rssService.DeleteRssAsync(urlFeed);

                var success = delete.ToBool()
                    ? "berhasil."
                    : "gagal. Mungkin RSS tersebut sudah di hapus atau belum di tambahkan";

                await _requestProvider.EditAsync($"Hapus {urlFeed} {success}");
            }
        }
    }
}