using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace WinTenBot.Helpers
{
    public static class CacheHelper
    {
        private static string workingDir = "Storage/Caches";

        public static async Task WriteCacheAsync(this DataTable dataTable, string fileJson)
        {
            var filePath = $"{workingDir}/{fileJson}";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var json = dataTable.ToJson(true);
            await json.ToFile(filePath);
            ConsoleHelper.WriteLine("Writing cache success..");
        }

        public static async Task<DataTable> ReadCacheAsync(this string fileJson)
        {
            var filePath = $"{workingDir}/{fileJson}";
            var json = await File.ReadAllTextAsync(filePath);
            var dataTable = json.ToDataTable();
            ConsoleHelper.WriteLine($"Loaded cache. Rows: {dataTable.Rows.Count} items");
            return dataTable;
        }
    }
}