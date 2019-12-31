using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class RssService:Query
    {
        private string baseTable = "rss_history";
        private string rssSettingTable = "rss_settings";
        
        private MySqlProvider _mySqlProvider;
        private Message _message;

        public RssService()
        {
            _mySqlProvider = new MySqlProvider();
        }

        public RssService(Message message)
        {
            _message = message;
            _mySqlProvider = new MySqlProvider();
        }
        public async Task<bool> IsExistInHistory(Dictionary<string,object> where)
        {
            // var where = new Dictionary<string,object>()
            // {
            //     {"chat_id",},
            //     {"url",url}
            // };
            // return await IsDataExist(baseTable, where);
            
            var query = $"SELECT * FROM {baseTable} " +
                        $"WHERE chat_id = {where["chat_id"]} AND url = '{where["url"]}'";
            
            var dTbl = await _mySqlProvider.ExecQueryAsync(query);
            
            return dTbl.Rows.Count > 0;
        }

        public async Task<bool> IsExistRssAsync(string urlFeed)
        {
            var where = new Dictionary<string,object>()
            {
                {"chat_id",_message.Chat.Id},
                {"url_feed", urlFeed}
            };
            
            var query = $"SELECT * FROM {rssSettingTable} " +
                        $"WHERE chat_id = {where["chat_id"]} AND url_feed = '{urlFeed}'";
            
            var dTbl = await _mySqlProvider.ExecQueryAsync(query);
            
            return dTbl.Rows.Count > 0;
        }

        public async Task<bool> SaveRssSettingAsync(Dictionary<string,object> data)
        {
            return await _mySqlProvider.Insert(rssSettingTable, data);
            // return await Insert(baseTable, data);
        }
        
        public async Task<bool> SaveRssAsync(Dictionary<string,object> data)
        {
            return await _mySqlProvider.Insert(baseTable, data);
            // return await Insert(baseTable, data);
        }

        public async Task<DataTable> GetRssSettingsAsync()
        {
            var sql = $"SELECT * FROM {rssSettingTable}";
            return await _mySqlProvider.ExecQueryAsync(sql);
        }
    }
}