using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using SqlKata.Extensions;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;
using Query = SqlKata.Query;

namespace WinTenBot.Handlers.Commands.Core
{
    public class TestCommand : CommandBase
    {
        private RssService _rssService;

        public TestCommand()
        {
        }
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

                // var sqlObj = new Query("rss_history").Where("chat_id","123");
                // QueryFactory sqlFactory = Bot.SqlKataFactory;
                // var compiler = new MySqlCompiler();
                // var sql = compiler.Compile(sqlObj);

                var data = await new Query("rss_history")
                    .Where("chat_id", chatId)
                    .ExecForMysql()
                    .GetAsync();

                // var asdf = data.ToDataTable();
                // ConsoleHelper.WriteLine(data.ToJson(true));
                ConsoleHelper.WriteLine(data.Count());
                // ConsoleHelper.WriteLine(asdf.Rows.Count);

                var isExistRss = await _rssService.IsExistRssAsync("https://blogs.windows.com/rss");
                ConsoleHelper.WriteLine($"IsExist: {isExistRss}");

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