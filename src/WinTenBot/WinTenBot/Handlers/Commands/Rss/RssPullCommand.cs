using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class RssPullCommand : CommandBase
    {
        // private ResponseProvider response;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            ChatHelper.Init(context);
            // response = new ResponseProvider(context);

            var chatId = ChatHelper.Message.Chat.Id.ToString();
            var isAdmin = await ChatHelper.IsAdminGroup();

            if (isAdmin || ChatHelper.IsPrivateChat())
            {
                
#pragma warning disable 4014
                Task.Run(async () =>
#pragma warning restore 4014
                {
                    Thread.CurrentThread.IsBackground = true;

                    // await "Sedang memeriksa RSS feed baru..".SendTextAsync();
                    await "Sedang memeriksa RSS feed baru..".SendTextAsync();

                    var newRssCount = await RssHelper.ExecBroadcasterAsync(chatId);
                    if (newRssCount == 0)
                    {
                        await "Tampaknya tidak ada RSS baru saat ini".EditAsync();
                    }
                    
                    ChatHelper.Close();
                    
                }, cancellationToken);
            }

        }
    }
}