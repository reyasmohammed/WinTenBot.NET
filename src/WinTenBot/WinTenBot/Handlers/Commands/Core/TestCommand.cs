using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
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
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
         _requestProvider = new RequestProvider(context);
            _rssService = new RssService(context.Update.Message);

            var chatId = _requestProvider.Message.Chat.Id;

            if (_requestProvider.Message.From.Id.IsSudoer())
            {
                Log.Information("Test started..");
                await _requestProvider.SendTextAsync("Sedang mengetes sesuatu");
                
                // var data = await new Query("rss_history")
                //     .Where("chat_id", chatId)
                //     .ExecForMysql()
                //     .GetAsync();
                //
                // var rssHistories = data
                //     .ToJson()
                //     .MapObject<List<RssHistory>>();
                //
                // ConsoleHelper.WriteLine(data.GetType());
                // // ConsoleHelper.WriteLine(data.ToJson(true));
                //
                // ConsoleHelper.WriteLine(rssHistories.GetType());
                // // ConsoleHelper.WriteLine(rssHistories.ToJson(true));
                //
                // ConsoleHelper.WriteLine("Test completed..");

                // await "This test".LogToChannel();

                // await RssHelper.SyncRssHistoryToCloud();
                await BotHelper.ClearLog();
                
                await _requestProvider.EditAsync("Selesai ngetest");
            }
            else
            {
                await _requestProvider.SendTextAsync("Unauthorized.");
            }
        }
    }
}