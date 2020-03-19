using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Serilog;

namespace WinTenBot.Helpers
{
    public static class CacheHelper
    {
        private static string workingDir = "Storage/Caches";

        public static async Task WriteCacheAsync(this object data, string fileJson, bool indented = true)
        {
            var filePath = $"{workingDir}/{fileJson}".EnsureDirectory();
            var json = data.ToJson(indented);
            
            await json.ToFile(filePath);
            Log.Information("Writing cache success..");
        }

        public static void BackgroundWriteCache(this object dataTable, string fileJson)
        {
            var jobId = BackgroundJob.Enqueue(() => WriteCacheAsync(dataTable, fileJson, true));
            Log.Information($"Background Write Cache scheduled with ID: {jobId}");
        }

        public static async Task<T> ReadCacheAsync<T>(this string fileJson)
        {
            var filePath = $"{workingDir}/{fileJson}";
            var json = await File.ReadAllTextAsync(filePath);
            var dataTable = json.MapObject<T>();
            
            // var dataTable = json.ToDataTable();
            Log.Information($"Loaded cache items");
            return dataTable;
        }

        public static bool IsFileCacheExist(this string fileName)
        {
            var filePath = $"{workingDir}/{fileName}";
            var isExist = File.Exists(filePath);
            Log.Information($"IsCache {fileName} Exist: {isExist}");
            
            return isExist;
        }

        public static void ClearCache(string keyword)
        {
            Log.Information($"Deleting caches. Keyword {keyword}");
            
            var listFile = Directory.GetFiles(workingDir);
            var listFiltered = listFile.Where(file => 
                file.Contains(keyword)
                );
            
            Log.Information($"Found cache target {listFiltered.Count()} of {listFile.Count()}");
        }
    }
}