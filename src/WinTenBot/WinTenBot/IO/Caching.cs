using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Serilog;
using WinTenBot.Common;

namespace WinTenBot.IO
{
    public static class Caching
    {
        private static string workingDir = "Storage/Caches";

        public static async Task WriteCacheAsync(this object data, string fileJson, bool indented = true)
        {
            var filePath = Dirs.EnsureDirectory($"{workingDir}/{fileJson}");
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
            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var dataTable = json.MapObject<T>();
            
            // var dataTable = json.ToDataTable();
            Log.Information($"Loaded cache items");
            return dataTable;
        }

        public static bool IsFileCacheExist(this string fileName)
        {
            var filePath = $"{workingDir}/{fileName}";
            var isExist = System.IO.File.Exists(filePath);
            Log.Information($"IsCache {fileName} Exist: {isExist}");
            
            return isExist;
        }

        public static void ClearCache(string keyword)
        {
            Log.Information($"Deleting caches. Keyword {keyword}");
            
            var listFile = System.IO.Directory.GetFiles(workingDir);
            var listFiltered = listFile.Where(file => 
                file.Contains(keyword)).ToArray();
            
            Log.Information($"Found cache target {listFiltered.Count()} of {listFile.Count()}");
            foreach (var file in listFiltered)
            {
                Log.Information($"Deleting {file}");
                System.IO.File.Delete(file);
            }
        }
        
        public static void ClearCacheOlderThan(string keyword, int days = 1)
        {
            Log.Information($"Deleting caches older than {days} days");
            
            var dirInfo = new DirectoryInfo(workingDir);
            var files = dirInfo.GetFiles();
            var filteredFiles = files.Where(fileInfo =>
                fileInfo.CreationTimeUtc < DateTime.UtcNow.AddDays(-days) &&
                fileInfo.Name.Contains(keyword)).ToArray();

            Log.Information($"Found cache target {filteredFiles.Count()} of {files.Count()}");

            foreach (FileInfo file in filteredFiles)
            {
                Log.Information($"Deleting {file.FullName}");
                System.IO.File.Delete(file.FullName);
            }
        }
    }
}