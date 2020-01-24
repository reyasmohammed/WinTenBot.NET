using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Migration;
using WinTenBot.Model;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class RssService
    {
        private string baseTable = "rss_history";
        private string rssSettingTable = "rss_settings";

        private Message _message;

        public RssService(Message message = null)
        {
            _message = message;

            // RssMigration.MigrateRssHistory();
        }

        public async Task<bool> IsExistInHistory(Dictionary<string, object> where)
        {
            // var isExist = await baseTable.IfTableExistAsync();
            // await isExist.MigrateRssHistory();
            // await baseTable.MigrateLocalStorage();


            var data = await new Query(baseTable)
                .Where(where)
                .ExecForSqLite(true)
                .GetAsync();

            ConsoleHelper.WriteLine($"Check RSS History: {data.Count().ToBool()}");
            return data.Any();
        }

        public async Task<bool> IsExistRssAsync(string urlFeed)
        {
            var data = await new Query(rssSettingTable)
                .Where("chat_id", _message.Chat.Id)
                .Where("url_feed", urlFeed)
                .ExecForMysql()
                .GetAsync();

            ConsoleHelper.WriteLine($"Check RSS Setting: {data.Count().ToBool()}");

            return data.Any();
        }

        public async Task<bool> SaveRssSettingAsync(Dictionary<string, object> data)
        {
            var insert = await new Query(rssSettingTable)
                .ExecForMysql()
                .InsertAsync(data);
            return insert.ToBool();
        }

        public async Task<bool> SaveRssHistoryAsync(Dictionary<string, object> data)
        {
            // var isExist = await baseTable.IfTableExistAsync();
            // await isExist.MigrateRssHistory();
            // await baseTable.MigrateLocalStorage();

            var insert = await new Query(baseTable)
                .ExecForSqLite(true)
                .InsertAsync(data);

            return insert.ToBool();
        }

        public async Task<List<RssSetting>> GetRssSettingsAsync(string chatId)
        {
            var data = await new Query(rssSettingTable)
                .Where("chat_id", chatId)
                .ExecForMysql()
                .GetAsync();

            var mapped = data.ToJson().MapObject<List<RssSetting>>();
            // ConsoleHelper.WriteLine(mapped.ToJson());

            ConsoleHelper.WriteLine($"Get RSS Settings: {data.Count()}");
            return mapped;

            // return data.ToJson().ToDataTable();
        }

        public async Task<List<RssSetting>> GetListChatIdAsync()
        {
            var data = await new Query(rssSettingTable)
                .Select("chat_id")
                .Distinct()
                .ExecForMysql()
                .GetAsync();

            var mapped = data.ToJson().MapObject<List<RssSetting>>();

            ConsoleHelper.WriteLine($"Get List ChatID: {data.Count()}");
            return mapped;

            // return data.ToJson().ToDataTable();
        }

        public async Task<bool> DeleteRssAsync(string urlFeed)
        {
            var delete = await new Query(rssSettingTable)
                .Where("chat_id", _message.Chat.Id)
                .Where("url_feed", urlFeed)
                .ExecForMysql()
                .DeleteAsync();

            $"Delete {urlFeed} status: {delete.ToBool()}".ToConsoleStamp();

            return delete.ToBool();
        }
    }
}