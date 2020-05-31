using System;
using System.Linq;
using Serilog;
using WinTenBot.Text;
using sysIO = System.IO;

namespace WinTenBot.IO
{
    public static class Directory
    {
        public static string EnsureDirectory(this string dirPath)
        {
            Log.Information($"EnsuringDir of {dirPath}");

            var path = sysIO.Path.GetDirectoryName(dirPath);
            if (!path.IsNullOrEmpty())
                sysIO.Directory.CreateDirectory(path);

            return dirPath;
        }

        public static string SanitizeSlash(this string path)
        {
            return path.Replace(@"\", "/", StringComparison.CurrentCulture)
                .Replace("\\", "/", StringComparison.CurrentCulture);
        }

        public static string GetDirectory(this string path)
        {
            return sysIO.Path.GetDirectoryName(path) ?? path;
        }

        public static void RemoveFiles(this string path, string filter)
        {
            Log.Information($"Deleting files in {path}");
            var files = sysIO.Directory.GetFiles(path)
                .Where(file => file.Contains(filter, StringComparison.CurrentCulture));
            
            foreach (string file in files)
            {
                sysIO.File.Delete(file);
            }
        }
    }
}