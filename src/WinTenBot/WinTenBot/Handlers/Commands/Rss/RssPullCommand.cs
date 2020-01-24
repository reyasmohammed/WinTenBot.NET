using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class RssPullCommand : CommandBase
    {
        private RequestProvider response;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            // ChatHelper.Init(context);
            response = new RequestProvider(context);

            // var chatId = ChatHelper.Message.Chat.Id.ToString();
            // var isAdmin = await ChatHelper.IsAdminGroup();

            var chatId = response.Message.Chat.Id.ToString();
            var isAdmin = await response.IsAdminGroup();

            if (isAdmin || response.IsPrivateChat())
            {
                
#pragma warning disable 4014
                Task.Run(async () =>
#pragma warning restore 4014
                {
                    // Thread.CurrentThread.IsBackground = true;

                    await response.SendTextAsync("Sedang memeriksa RSS feed baru..");
                    // await "Sedang memeriksa RSS feed baru..".SendTextAsync();

                    var newRssCount =  await RssHelper.ExecBroadcasterAsync(chatId);
                    if (newRssCount == 0)
                    {
                        await response.EditAsync("Tampaknya tidak ada RSS baru saat ini");
                        // await "Tampaknya tidak ada RSS baru saat ini".EditAsync();
                    }
                    
                    // ChatHelper.Close();
                    
                }, cancellationToken);
            }

        }
    }
}