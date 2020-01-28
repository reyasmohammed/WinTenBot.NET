using System;
using System.IO;
using Serilog;

namespace WinTenBot.Helpers
{
    public static class IoHelper
    {
        public static void DeleteFile(this string filePath){
            if (!File.Exists(filePath)) return;
            try
            {
                Log.Information($"Deleting {filePath}");
                File.Delete(filePath);
                Log.Information($"File {filePath} deleted successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex,$"Error Deleting file {filePath}");
            }
        }
    }
}