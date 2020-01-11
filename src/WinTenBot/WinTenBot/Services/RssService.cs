using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class RssService
    {
        private string baseTable = "rss_history";
        private string rssSettingTable = "rss_settings";
        
        private Message _message;

        public RssService()
        {
            // _mySqlProvider = new MySqlProvider();
        }

        public RssService(Message message)
        {
            _message = message;
            // _mySqlProvider = new MySqlProvider();
        }
        public async Task<bool> IsExistInHistory(Dictionary<string,object> where)
        {
            var data = await new Query(baseTable)
                .Where(where)
                .ExecForMysql()
                .GetAsync();

            ConsoleHelper.WriteLine($"Check RSS History: {data.Count().ToBool()}");
            return data.Any();
        }

        public async Task<bool> IsExistRssAsync(string urlFeed)
        {
            var data = await new Query(rssSettingTable)
                .Where("chat_id",_message.Chat.Id)
                .Where("url_feed",urlFeed)
                .ExecForMysql()
                .GetAsync();
            
            ConsoleHelper.WriteLine($"Check RSS Setting: {data.Count().ToBool()}");

            return data.Any();
        }

        public async Task<bool> SaveRssSettingAsync(Dictionary<string,object> data)
        {
            var insert = await new Query(rssSettingTable)
                .ExecForMysql()
                .InsertAsync(data);
            return insert.ToBool();
        }
        
        public async Task<bool> SaveRssAsync(Dictionary<string,object> data)
        {
            var insert = await new Query(baseTable)
                .ExecForMysql()
                .InsertAsync(data);

            return insert.ToBool();
        }

        public async Task<DataTable> GetRssSettingsAsync(string chatId)
        {
            var data =  await new Query(rssSettingTable)
                .Where("chat_id", chatId)
                .ExecForMysql()
                .GetAsync();
            
            ConsoleHelper.WriteLine($"Get RSS Settings: {data.Count()}");

            return data.ToJson().ToDataTable();
        }

        public async Task<DataTable> GetListChatIdAsync()
        {
            var data = await new Query(rssSettingTable)
                .Select("chat_id")
                .ExecForMysql()
                .GetAsync();
            
            ConsoleHelper.WriteLine($"Get List ChatID: {data.Count()}");

            return data.ToJson().ToDataTable();
        }
    }
}