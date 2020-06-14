﻿using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;
using WinTenBot.Model;

namespace WinTenBot.IO
{
    public static class Files
    {
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
        
        public static async Task WriteTextAsync(this string content, string filePath)
        {
            var cachePath = BotSettings.PathCache;

            filePath = $"{cachePath}/{filePath}";
            Log.Information($"Writing content to {filePath}");

            Path.GetDirectoryName(filePath).EnsureDirectory();

            await File.WriteAllTextAsync(filePath, content)
                .ConfigureAwait(false);
            
            Log.Information("Writing file success..");
        }

        public static void WriteText(this string content, string filePath)
        {
            var cachePath = BotSettings.PathCache;
            
            filePath = $"{cachePath}/{filePath}";
            Log.Information($"Writing content to {filePath}");

            Path.GetDirectoryName(filePath).EnsureDirectory();

            File.WriteAllText(filePath, content);
            Log.Information("Writing file success..");
        }
    }
}