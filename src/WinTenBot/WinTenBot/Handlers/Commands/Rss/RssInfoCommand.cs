using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MySqlX.XDevAPI.Relational;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class RssInfoCommand:CommandBase
    {
        private RssService _rssService;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _rssService = new RssService(ChatHelper.Message);
            
            ChatHelper.Init(context);

            var chatId = ChatHelper.Message.Chat.Id.ToString();
            var isAdmin = await ChatHelper.IsAdminGroup();
            
            if (isAdmin || ChatHelper.IsPrivateChat())
            {
                await "Sedang meload data..".SendTextAsync();
                var rssData = await _rssService.GetRssSettingsAsync(chatId);
                
                var sendText = $"List RSS {rssData.Rows.Count} items.";
                int num = 1;
                foreach (DataRow rss in rssData.Rows)
                {
                    var urlFeed = rss["url_feed"].ToString();
                    sendText += $"\n{num++}. {urlFeed}";
                }

                await sendText.EditAsync();
            }
            else
            {
                await "Kamu bukan admin, atau kamu bisa mengaturnya di japri ".SendTextAsync();
            }
            
            ChatHelper.Close();
        }
    }
}