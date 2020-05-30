﻿using System;
using System.IO;
using Serilog;
using WinTenBot.Helpers;

namespace WinTenBot.IO
{
    public static class Directory
    {
        public static string EnsureDirectory(this string dirPath)
        {
            Log.Information($"EnsuringDir of {dirPath}");

            var path = Path.GetDirectoryName(dirPath);
            if (!path.IsNullOrEmpty())
                System.IO.Directory.CreateDirectory(path);

            return dirPath;
        }

        public static string SanitizeSlash(this string path)
        {
            return path.Replace(@"\", "/", StringComparison.CurrentCulture)
                .Replace("\\", "/", StringComparison.CurrentCulture);
        }
    }
}