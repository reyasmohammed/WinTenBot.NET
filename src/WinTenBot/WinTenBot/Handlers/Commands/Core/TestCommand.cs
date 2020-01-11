using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Core
{
    public class TestCommand : CommandBase
    {
        private RssService _rssService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            ChatHelper.Init(context);
            _rssService = new RssService(context.Update.Message);

            var chatId = ChatHelper.Message.Chat.Id;

            if (ChatHelper.Message.From.Id.IsSudoer())
            {
                ConsoleHelper.WriteLine("Test started..");
                await "Sedang mengetes sesuatu".SendTextAsync();
                
                var data = await new Query("rss_history")
                    .Where("chat_id", chatId)
                    .ExecForMysql()
                    .GetAsync();
                
                var rssHistories = data
                    .ToJson()
                    .MapObject<List<RssHistory>>();

                ConsoleHelper.WriteLine(data.GetType());
                ConsoleHelper.WriteLine(data.ToJson(true));
                
                ConsoleHelper.WriteLine(rssHistories.GetType());
                ConsoleHelper.WriteLine(rssHistories.ToJson(true));
                
                ConsoleHelper.WriteLine("Test completed..");
                await "Selesai ngetest".EditAsync();
            }
            else
            {
                await "Unauthorized.".SendTextAsync();
            }

            ChatHelper.Close();
        }
    }
}