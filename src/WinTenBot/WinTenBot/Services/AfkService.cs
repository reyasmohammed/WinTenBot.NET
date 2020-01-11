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
    public class AfkService
    {
        private const string BaseTable = "afk";
        private const string FileJson = "afk.json";

        public async Task<bool> IsExist(string key, string value)
        {
            var data = await new Query(BaseTable)
                .Where(key, value)
                .ExecForMysql()
                .GetAsync();
            
            ConsoleHelper.WriteLine($"Check AFK Exist: {data.Count().ToBool()}");
            return data.Any();
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

            var insert =0;
            
            var checkExist = await new Query(BaseTable)
                .Where(where)
                .ExecForMysql()
                .GetAsync();

            var isExist = checkExist.Any();
            
            if (isExist)
            {
                insert = await new Query(BaseTable)
                    .Where(where)
                    .ExecForMysql()
                    .UpdateAsync(data);
            }
            else
            {
                insert = await new Query(BaseTable)
                    .ExecForMysql()
                    .InsertAsync(data);
            }

            ConsoleHelper.WriteLine($"SaveAfk: {insert}");
        }

        public async Task<DataTable> GetAllAfk()
        {
            var data = await new Query(BaseTable)
                .ExecForMysql()
                .GetAsync();

            var dataTable = data.ToJson().ToDataTable();
            return dataTable;
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