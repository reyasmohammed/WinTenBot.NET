using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Hosting.Internal;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

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
                Task.Run(async () =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    // await "Sedang memeriksa RSS feed baru..".SendTextAsync();
                    await ChatHelper.SendTextAsync("Sedang memeriksa RSS feed baru..");

                    var newRssCount = RssHelper.ExecBroadcasterAsync(chatId).Result;
                    if (newRssCount == 0)
                    {
                        await ChatHelper.EditAsync("Tampaknya tidak ada RSS baru saat ini");
                    }
                    
                    ChatHelper.Close();
                    
                }, cancellationToken);
            }

        }
    }
}