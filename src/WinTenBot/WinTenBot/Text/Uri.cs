using System.Net;
using Flurl;
using Serilog;

namespace WinTenBot.Text
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