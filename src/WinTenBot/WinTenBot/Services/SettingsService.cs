using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class SettingsService : Query
    {
        private readonly MySqlProvider _mySqlProvider;
        private string baseTable = "group_settings";
        private Chat Chat { get; set; }

        public SettingsService(Chat chat)
        {
            _mySqlProvider = new MySqlProvider();
            Chat = chat;
        }

        public async Task<DataTable> GetSettingByGroup()
        {
            var where = new Dictionary<string, object>()
            {
                {"chat_id",Chat.Id}
            };

            var isExist = await IsDataExist(baseTable, where);
            ConsoleHelper.WriteLine($"Group setting IsExist: {isExist}");
            if (!isExist)
            {
                ConsoleHelper.WriteLine($"Inserting new data for {Chat.Id}");
                var insert = new Dictionary<string, object>()
                {
                    {"chat_id", Chat.Id},
                    {"chat_title",Chat.Title}
                };
                await Insert(baseTable, insert);
            }

            var sql = $"SELECT * FROM {baseTable} WHERE chat_id = '{Chat.Id}'";
            var data = await _mySqlProvider.ExecQueryAsync(sql);
            return data;
        }

        public async Task<ChatSettings> GetMappedSettingsByGroup()
        {
            var chatSettings = new ChatSettings();
            var settings = await GetSettingByGroup();
            
            if (settings.Rows.Count <= 0) return chatSettings;
            
            chatSettings.ChatId = Chat.Id;
            chatSettings.WelcomeMessage = settings.Rows[0]["welcome_message"].ToString(); 
            chatSettings.WelcomeButton = settings.Rows[0]["welcome_button"].ToString();
            chatSettings.WelcomeMedia = settings.Rows[0]["welcome_media"].ToString();
            chatSettings.WelcomeMediaType = settings.Rows[0]["welcome_media_type"].ToString();

            return chatSettings;
        }

        public async Task UpdateCell(string key, object value)
        {
            var sql = $"UPDATE {baseTable} " +
                      $"SET {key} = '{value}' " +
                      $"WHERE chat_id = '{Chat.Id}'";

            await _mySqlProvider.ExecNonQueryAsync(sql);
        }

        public async Task UpdateCache()
        {
            var data = await GetSettingByGroup();
            await data.WriteCacheAsync($"{Chat.Id}/settings.json");
        }
    }
}