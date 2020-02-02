using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class RssInfoCommand : CommandBase
    {
        private RssService _rssService;
        private RequestProvider _requestProvider;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            _rssService = new RssService(context.Update.Message);

            var chatId = _requestProvider.Message.Chat.Id.ToString();
            var isAdmin = await _requestProvider.IsAdminGroup();

            if (isAdmin || _requestProvider.IsPrivateChat())
            {
                await _requestProvider.SendTextAsync("🔄 Sedang meload data..");
                var rssData = await _rssService.GetRssSettingsAsync(chatId);

                var sendText = $"📚 <b>List RSS</b>: {rssData.Count} Items.";
                int num = 1;
                foreach (RssSetting rss in rssData)
                {
                    // var urlFeed = rss["url_feed"].ToString();
                    sendText += $"\n{num++}. {rss.UrlFeed}";
                }

                if (rssData.Count == 0)
                {
                    sendText += "\n\nSepertinya kamu belum menambahkan RSS disini. " +
                                "Kamu dapat menambahkan RSS dengan jumlah tidak terbatas!" +
                                "\nGunakan <code>/setrss https://link_rss_nya</code> untuk menambahkan.";
                }

                await _requestProvider.EditAsync(sendText);
            }
            else
            {
                await _requestProvider.SendTextAsync("Kamu bukan admin, atau kamu bisa mengaturnya di japri ");
            }
        }
    }
}