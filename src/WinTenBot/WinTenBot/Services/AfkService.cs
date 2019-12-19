using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class AfkService : Query
    {
        private readonly MySqlProvider _mySqlProvider;
        private const string BaseTable = "afk";
        private const string FileJson = "afk.json";

        public AfkService()
        {
            _mySqlProvider = new MySqlProvider();
        }

        public async Task<bool> IsExist(string key, string value)
        {
            var sql = $"SELECT * FROM {BaseTable} WHERE {key} = '{value}'";
            var data = await _mySqlProvider.ExecQueryAsync(sql);
            return data.Rows.Count > 0;
        }

        public async Task<bool> IsExistInCache(string key, string val)
        {
            var data = await ReadCacheAsync();
            var search = data.AsEnumerable()
                .Where(row => row.Field<string>(key) == val);
            if (!search.Any()) return false;

            var filtered = search.CopyToDataTable();
            ConsoleHelper.WriteLine($"AFK found in Caches: {filtered.ToJson()}");
            return true;
        }

        public async Task<bool> IsAfkAsync(Message message)
        {
            var isAfk = await IsExistInCache("user_id", message.From.Id.ToString());

            var afkCache = await ReadCacheAsync();
            var filteredAfk = afkCache.AsEnumerable()
                .Where(row => row.Field<object>("is_afk").ToBool() == true
                              && row.Field<string>("chat_id").ToString() == message.Chat.Id.ToString()
                              && row.Field<string>("user_id").ToString() == message.From.Id.ToString());
            if (!filteredAfk.Any()) isAfk = false;

            ConsoleHelper.WriteLine($"IsAfk: {isAfk}");
            return isAfk;
        }

        public async Task SaveAsync(Dictionary<string, object> data)
        {
            ConsoleHelper.WriteLine(data.ToJson());
            var where = new Dictionary<string, object>()
            {
                {"user_id", data["user_id"]}
            };

            var insert = false;
            var isExist = await IsDataExist(BaseTable, where);
            if (isExist)
            {
                insert = await Update(BaseTable, data, where);
            }
            else
            {
                insert = await _mySqlProvider.Insert(BaseTable, data);
            }

            ConsoleHelper.WriteLine($"SaveAfk: {insert}");
        }

        public async Task UpdateCell(Message message, string key, object value)
        {
            var sql = $"UPDATE {BaseTable} " +
                      $"SET {key} = '{value}' " +
                      $"WHERE chat_id = '{message.Chat.Id}' " +
                      $"AND user_id ='{message.From.Id}'";

            await _mySqlProvider.ExecNonQueryAsync(sql);
        }

        public async Task<DataTable> GetAllAfk()
        {
            var sql = $"SELECT * FROM {BaseTable}";
            var data = await _mySqlProvider.ExecQueryAsync(sql);
            return data;
        }

        public async Task UpdateCacheAsync()
        {
            var data = await GetAllAfk();
            ConsoleHelper.WriteLine($"Updating AFK caches to {FileJson}");

            await data.WriteCacheAsync(FileJson);
        }

        public async Task<DataTable> ReadCacheAsync()
        {
            var dataTable = await FileJson.ReadCacheAsync();
            return dataTable;
        }
    }
}