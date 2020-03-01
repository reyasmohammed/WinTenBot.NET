using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;

namespace WinTenBot.Helpers
{
    public static class IoHelper
    {
        public static string BaseDirectory { get; set; } = "Storage/Caches";

        public static void DeleteFile(this string filePath)
        {
            if (!File.Exists(filePath)) return;

            try
            {
                Log.Information($"Deleting {filePath}");
                File.Delete(filePath);
                Log.Information($"File {filePath} deleted successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error Deleting file {filePath}");
            }
        }

        public static string EnsureDirectory(this string dirPath)
        {
            if (!dirPath.IsNullOrEmpty())
                Directory.CreateDirectory(dirPath);

            return dirPath;
        }

        public static async Task WriteTextAsync(this string content, string filePath)
        {
            filePath = $"{BaseDirectory}/{filePath}";
            Log.Information($"Writing content to {filePath}");

            Path.GetDirectoryName(filePath).EnsureDirectory();

            await File.WriteAllTextAsync(filePath, content);
            Log.Information("Writing file success..");
        }

        public static void WriteText(this string content, string filePath)
        {
            filePath = $"{BaseDirectory}/{filePath}";
            Log.Information($"Writing content to {filePath}");

            Path.GetDirectoryName(filePath).EnsureDirectory();

            File.WriteAllText(filePath, content);
            Log.Information("Writing file success..");
        }
    }
}