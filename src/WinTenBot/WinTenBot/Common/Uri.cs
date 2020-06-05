using System.IO;
using System.Net;
using Flurl;
using Serilog;
using WinTenBot.IO;
using WinTenBot.Model;

namespace WinTenBot.Common
{
    public static class Uri
    {
        public static string GenerateUrlQrApi(this string data)
        {
            return $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&margin=10&data={Url.Encode(data)}";
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

        public static Url ParseUrl(this string urlPath)
        {
            var url = new Url(urlPath);

            return url;
        }

        public static bool IsValidUrl(this string urlPath)
        {
            return Url.IsValid(urlPath);
        }
    }
}