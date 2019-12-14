using System.Data;
using System.IO;
using System.Threading.Tasks;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class ElasticSecurityService
    {
        private string fbanTable = "fbans";
        private string fileJson = "fban_user.json";
        private MySqlProvider _mySqlProvider;

        public ElasticSecurityService()
        {
            _mySqlProvider = new MySqlProvider();
        }

        public async Task<bool> IsExistInCache(string key, string val)
        {
            var data = await ReadCacheAsync();
            var search = data.AsEnumerable()
                .Where(row => row.Field<string>(key) == val);
            var filtered = search.CopyToDataTable();

            ConsoleHelper.WriteLine($"Caches found: {filtered.ToJson()}");
            return filtered.Rows.Count > 0;
        }

        public async Task UpdateCacheAsync()
        {
            var sql = $"SELECT * FROM {fbanTable}";
            var data = await _mySqlProvider.ExecQueryAsync(sql);

            ConsoleHelper.WriteLine($"Updating Media Filter caches to {fileJson}");
            await data.WriteCacheAsync(fileJson);

//            var json = data.ToJson(true);
//            await json.ToFile(fileJson);
//            ConsoleHelper.WriteLine("Writing success..");
        }

        public async Task<DataTable> ReadCacheAsync()
        {
            var dataTable = await fileJson.ReadCacheAsync();
            return dataTable;

//            var json = await File.ReadAllTextAsync(fileJson);
//            var dataTable = json.ToDataTable();
//            ConsoleHelper.WriteLine($"Loaded cache. Rows: {dataTable.Rows.Count}");
//            return dataTable;
        }
    }
}