using System;
using System.Threading.Tasks;
using Serilog;
using WinTenBot.Model;
using SysIO = System.IO;

namespace WinTenBot.IO
{
    public static class File
    {
        public static void DeleteFile(this string filePath)
        {
            if (!SysIO.File.Exists(filePath)) return;

            try
            {
                Log.Information($"Deleting {filePath}");
                SysIO.File.Delete(filePath);

                Log.Information($"File {filePath} deleted successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error Deleting file {filePath}");
            }
        }
        
        public static async Task WriteTextAsync(this string content, string filePath)
        {
            var cachePath = BotSettings.PathCache;

            filePath = $"{cachePath}/{filePath}";
            Log.Information($"Writing content to {filePath}");

            SysIO.Path.GetDirectoryName(filePath).EnsureDirectory();

            await System.IO.File.WriteAllTextAsync(filePath, content)
                .ConfigureAwait(false);
            
            Log.Information("Writing file success..");
        }

        public static void WriteText(this string content, string filePath)
        {
            var cachePath = BotSettings.PathCache;
            
            filePath = $"{cachePath}/{filePath}";
            Log.Information($"Writing content to {filePath}");

            SysIO.Path.GetDirectoryName(filePath).EnsureDirectory();

            System.IO.File.WriteAllText(filePath, content);
            Log.Information("Writing file success..");
        }
    }
}