using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class RssInfoCommand : CommandBase
    {
        private RssService _rssService;
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            _rssService = new RssService(context.Update.Message);

            var chatId = _telegramProvider.Message.Chat.Id;
            var isAdmin = await _telegramProvider.IsAdminGroup();

            if (isAdmin || _telegramProvider.IsPrivateChat())
            {
                await _telegramProvider.SendTextAsync("🔄 Sedang meload data..");
                var rssData = await _rssService.GetRssSettingsAsync(chatId);
                var rssCount = rssData.Count();

                var sendText = $"📚 <b>List RSS</b>: {rssCount} Items.";
                int num = 1;
                foreach (RssSetting rss in rssData)
                {
                    // var urlFeed = rss["url_feed"].ToString();
                    sendText += $"\n{num++}. {rss.UrlFeed}";
                }

                if (rssCount == 0)
                {
                    sendText += "\n\nSepertinya kamu belum menambahkan RSS disini. " +
                                "Kamu dapat menambahkan RSS dengan jumlah tidak terbatas!" +
                                "\nGunakan <code>/setrss https://link_rss_nya</code> untuk menambahkan.";
                }

                await _telegramProvider.EditAsync(sendText);
            }
            else
            {
                await _telegramProvider.SendTextAsync("Kamu bukan admin, atau kamu bisa mengaturnya di japri ");
            }
        }
    }
}