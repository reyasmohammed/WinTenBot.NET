using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class RssPullCommand:CommandBase
    {
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        { 
            ChatHelper.Init(context);
            
            var chatId = ChatHelper.Message.Chat.Id.ToString();
            var isAdmin = await ChatHelper.IsAdminGroup();

            if (isAdmin || ChatHelper.IsPrivateChat())
            {
                await "Sedang mengambil feed..".SendTextAsync();
                
                var newRssCount = await RssHelper.ExecSchedulerAsync(chatId);
                if (newRssCount == 0)
                {
                    await "Tampaknya tidak ada RSS baru saat ini".EditAsync();
                }
            }
            
            ChatHelper.Close();
        }
    }
}