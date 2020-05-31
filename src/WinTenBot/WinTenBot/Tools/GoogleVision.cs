using System.Collections.Generic;
using Google.Cloud.Vision.V1;
using Serilog;
using WinTenBot.IO;
using WinTenBot.Model;
using WinTenBot.Text;

namespace WinTenBot.Tools
{
    public static class GoogleVision
    {
        private static ImageAnnotatorClient MakeClient()
        {
            var credPath = BotSettings.GoogleCloudCredentialsPath.SanitizeSlash();
            Log.Information($"Instantiates a client, cred {credPath}");
            var clientBuilder = new ImageAnnotatorClientBuilder
            {
                CredentialsPath = credPath
            };

            var client = clientBuilder.Build();
            return client;
        }

        public static string ScanText(string filePath)
        {
            Log.Information($"GoogleVision detect text {filePath}");

            var client = MakeClient();
            Log.Information("Load the image file into memory");
            var image = Image.FromFile(filePath);

            Log.Information("Performs text detection on the image file");
            var response = client.DetectText(image);
            
            Log.Information($"ResponseCount: {response.Count}");
            if (response.Count != 0) return response[0].Description;
            
            Log.Information("Seem no string result.");
            return null;
            
            // PrintAnnotation(response);
        }

        private static void PrintAnnotation(IReadOnlyList<EntityAnnotation> entityAnnotations)
        {
            foreach (var annotation in entityAnnotations)
            {
                // if (annotation.Description != null)
                Log.Information($"Annotation {annotation.ToJson(true)}");
                Log.Information($"Desc {annotation.Score} - {annotation.Description}");
            }
        }
    }
}