using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class SettingsService
    {
        private string baseTable = "group_settings";
        private Chat Chat { get; set; }

        public SettingsService(Chat chat)
        {
            Chat = chat;
        }

        public async Task<bool> IsSettingExist()
        {
            var where = new Dictionary<string, object>() {{"chat_id", Chat.Id}};
            
            var data = await new Query(baseTable)
                .Where(where)
                .ExecForMysql()
                .GetAsync();
            var isExist = data.Any();
            
            ConsoleHelper.WriteLine($"Group setting IsExist: {isExist}");
            return isExist;
        }

        public async Task<ChatSetting> GetSettingByGroup()
        {
            var where = new Dictionary<string, object>()
            {
                {"chat_id", Chat.Id}
            };

            var data = await new Query(baseTable)
                .Where(where)
                .ExecForMysql()
                .GetAsync();

            var mapped = data.ToJson().MapObject<List<ChatSetting>>();

            return mapped.FirstOrDefault();
        }

        public async Task SaveSettingsAsync(Dictionary<string, object> data)
        {
            var where = new Dictionary<string, object>() {{"chat_id", data["chat_id"]}};
            // var isExist = await IsDataExist(baseTable, where);
            
            var check = await new Query(baseTable)
                .Where(where)
                .ExecForMysql()
                .GetAsync();
            var isExist = check.Any();
            
            ConsoleHelper.WriteLine($"Group setting IsExist: {isExist}");
            if (!isExist)
            {
                ConsoleHelper.WriteLine($"Inserting new data for {Chat.Id}");
                // await Insert(baseTable, data);
                
                var insert = await new Query(baseTable)
                    .ExecForMysql()
                    .InsertAsync(data);
            }
            else
            {
                ConsoleHelper.WriteLine($"Updating data for {Chat.Id}");
                // await Update(baseTable, data, where);
                
                var insert = await new Query(baseTable)
                    .Where(where)
                    .ExecForMysql()
                    .UpdateAsync(data);
            }
        }

        public async Task UpdateCell(string key, object value)
        {
            var where = new Dictionary<string, object>() {{"chat_id", Chat.Id}};
            var data = new Dictionary<string, object>() {{key, value}};
            
            await new Query(baseTable)
                .Where(where)
                .ExecForMysql()
                .UpdateAsync(data);
        }

        public async Task UpdateCache()
        {
            var data = await GetSettingByGroup();
            await data.WriteCacheAsync($"{Chat.Id}/settings.json");
        }
    }
}