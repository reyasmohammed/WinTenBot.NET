using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class ElasticSecurityService
    {
        private string fbanTable = "fbans";
        private string fileJson = "fban_user.json";
        // private MySqlProvider _mySqlProvider;
        private readonly Message _message;

        public ElasticSecurityService(Message message)
        {
            _message = message;
            // _mySqlProvider = new MySqlProvider();
        }

        public async Task<bool> IsExist(int userId)
        {
            var where = new Dictionary<string, object>()
            {
                {"user_id", userId}
            };
            // return await IsDataExist(fbanTable, where);
            
            var query = await new Query(fbanTable)
                .ExecForMysql(true)
                .Where(where)
                .GetAsync();
            var isBan = query.Any();
            Log.Information($"{userId} IsBan: {isBan}");
            
            return isBan;
        }

        public async Task<bool> IsExistInCache(int userId)
        {
            var data = await ReadCacheAsync();
            DataTable filtered = new DataTable(null);
            
            Log.Information($"Checking {userId} in Global Ban Cache");
            var search = data.AsEnumerable()
                .Where(row => row.Field<string>("user_id") == userId.ToString());
            if (search.Any())
            {
                filtered = search.CopyToDataTable();
            }

            Log.Information($"Caches found: {filtered.ToJson()}");
            return filtered.Rows.Count > 0;
        }

        public async Task<bool> SaveBanAsync(Dictionary<string, object> data)
        {
            Log.Information($"Inserting new data for {_message.Chat.Id}");
            var query = await new Query(fbanTable)
                .ExecForMysql(true)
                .InsertAsync(data);

            return query > 0;
            // return await Insert(fbanTable, data);
        }

        public async Task<bool> DeleteBanAsync(int userId)
        {
            var where = new Dictionary<string, object>() {{"user_id",userId}};
            var delete = await new Query(fbanTable)
                .ExecForMysql(true)
                .Where(where)
                .DeleteAsync();

            return delete > 0;
            // return await Delete(fbanTable, where);
        }

        public async Task<DataTable> GetGlobalBanAll()
        {
            var query = await new Query(fbanTable)
                .ExecForMysql(true)
                .GetAsync();

            var data = query.ToJson().MapObject<DataTable>();
            // var sql = $"SELECT * FROM {fbanTable}";
            // var data = await _mySqlProvider.ExecQueryAsync(sql);
            return data;
        }

        public async Task UpdateCacheAsync()
        {
            var data = await GetGlobalBanAll();

            Log.Information($"Updating Global Ban caches to {fileJson}");
            await data.WriteCacheAsync(fileJson);
        }

        public async Task<DataTable> ReadCacheAsync()
        {
            var dataTable = await fileJson.ReadCacheAsync();
            return dataTable;
        }
    }
}