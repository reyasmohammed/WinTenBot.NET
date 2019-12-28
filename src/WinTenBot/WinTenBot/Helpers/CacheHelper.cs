using System.Data;
using System.IO;
using System.Threading.Tasks;
using Hangfire;

namespace WinTenBot.Helpers
{
    public static class CacheHelper
    {
        private static string workingDir = "Storage/Caches";

        public static async Task WriteCacheAsync(this DataTable dataTable, string fileJson, bool indented = true)
        {
            var filePath = $"{workingDir}/{fileJson}";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var json = dataTable.ToJson(indented);
            await json.ToFile(filePath);
            ConsoleHelper.WriteLine("Writing cache success..");
        }

        public static void BackgroundWriteCache(this DataTable dataTable, string fileJson)
        {
            var jobId = BackgroundJob.Enqueue(() => WriteCacheAsync(dataTable, fileJson, true));
            ConsoleHelper.WriteLine($"Background Write Cache scheduled with ID: {jobId}");
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