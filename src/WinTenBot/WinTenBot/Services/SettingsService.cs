using System.Data;
using System.Threading.Tasks;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class SettingsService
    {
        private readonly MySqlProvider _mySqlProvider;
        public long ChatId { get; set; }
        
        public SettingsService()
        {
            _mySqlProvider = new MySqlProvider();
        }
        
        public async Task<DataTable> GetSettingByGroup(long chatId)
        {
            var sql = $"SELECT * FROM group_settings WHERE chat_id = '{chatId}'";
            var data = await _mySqlProvider.ExecQueryAsync(sql);
            return data;
        }

        public async Task UpdateCell(long chatId, string key, object value)
        {
            var sql = $"UPDATE group_settings " +
                      $"SET {key} = '{value}' " +
                      $"WHERE chat_id = '{chatId}'";

            await _mySqlProvider.ExecNonQueryAsync(sql);
        }

        public async Task UpdateCache()
        {
            var data = await GetSettingByGroup(ChatId);
            await data.WriteCacheAsync($"{ChatId}/settings.json");
        }
    }
}