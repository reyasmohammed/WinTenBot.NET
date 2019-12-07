using System.Data;
using System.Threading.Tasks;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class SettingsService
    {
        private readonly MySqlProvider _mySqlProvider;
        public SettingsService()
        {
            _mySqlProvider = new MySqlProvider();
        }
        
        public async Task<DataTable> GetSettingByGrup(long chatId)
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
    }
}