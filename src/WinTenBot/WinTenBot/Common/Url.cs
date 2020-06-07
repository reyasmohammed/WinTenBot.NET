using System;
using System.IO;
using System.Net;
using Flurl;
using Serilog;
using WinTenBot.IO;
using WinTenBot.Model;

namespace WinTenBot.Common
{
    public static class Url
    {
        public static Uri GenerateUrlQrApi(this string data)
        {
            var baseUrl = "https://api.qrserver.com";
            return new Uri($"{baseUrl}/v1/create-qr-code/?size=300x300&margin=10&data={Flurl.Url.Encode(data)}");
        }

        public static void SaveUrlTo(this string remoteFileUrl, string localFileName)
        {
            var webClient = new WebClient();

            Log.Information($"Saving {remoteFileUrl} to {localFileName}");
            webClient.DownloadFile(remoteFileUrl, localFileName);
            webClient.Dispose();
        }

        public static string SaveToCache(this string remoteFileUrl, string localFileName)
        {
            var webClient = new WebClient();

            var cachePath = BotSettings.PathCache;
            var localPath = Path.Combine(cachePath, localFileName).EnsureDirectory();

            Log.Information($"Saving {remoteFileUrl} to {localPath}");
            webClient.DownloadFile(remoteFileUrl, localPath);
            webClient.Dispose();

            return localPath;
        }

        public static Flurl.Url ParseUrl(this string urlPath)
        {
            var url = new Flurl.Url(urlPath);

            return url;
        }

        public static bool IsValidUrl(this string urlPath)
        {
            return Flurl.Url.IsValid(urlPath);
        }
    }
}