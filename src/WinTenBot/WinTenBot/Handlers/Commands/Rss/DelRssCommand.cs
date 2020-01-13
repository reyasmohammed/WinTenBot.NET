using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class DelRssCommand : CommandBase
    {
        private RssService _rssService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            ChatHelper.Init(context);
            _rssService = new RssService(context.Update.Message);

            var isAdminOrPrivateChat = await ChatHelper.IsAdminOrPrivateChat();
            if (isAdminOrPrivateChat)
            {
                var urlFeed = ChatHelper.Message.Text.GetTextWithoutCmd();

                await $"Sedang menghapus {urlFeed}".SendTextAsync();

                var delete = await _rssService.DeleteRssAsync(urlFeed);

                var success = delete.ToBool()
                    ? "berhasil."
                    : "gagal. Mungkin RSS tersebut sudah di hapus atau belum di tambahkan";

                await $"Hapus {urlFeed} {success}".EditAsync();
            }


            ChatHelper.Close();
        }
    }
}