using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using Serilog;
using Tesseract;
using WinTenBot.Helpers;
using WinTenBot.Model;

namespace WinTenBot.Providers
{
    public class TesseractProvider
    {
        public static string ScanImage(string fileName)
        {
            string result = "";
            Log.Information($"Scanning {fileName}");
            using (var engine = new TesseractEngine(BotSettings.TesseractTrainedData, "ind",
                EngineMode.TesseractOnly))
            {
                using (var img = Pix.LoadFromFile(fileName))
                {
                    var page = engine.Process(img, PageSegMode.Auto);
                    result = page.GetText();
                    Log.Information("Scan complete.");
                }
            }

            return result.IsNullOrEmpty() ? "Return empty." : result;
        }

        public static async Task<string> OcrSpace(string filePath)
        {
            var result = "";
            try
            {
                var fs = File.OpenRead(filePath);
                var url = "https://api.ocr.space/Parse/Image";
                var ocrKey = BotSettings.OcrSpaceKey;

                Log.Information($"Sending {filePath} to {url}");
                var post = await url.PostMultipartAsync(post =>
                    post.AddFile("image", fs, Path.GetFileName(filePath))
                        .AddString("apikey", ocrKey)
                        .AddString("language", "eng"));

                Log.Information("Reading result..");
                var json = await post.Content.ReadAsStringAsync();
                var map = JsonConvert.DeserializeObject<OcrSpace>(json);

                if (map.OCRExitCode == 1)
                {
                    result = map.ParsedResults.Aggregate(result, (current, t) => current + t.ParsedText);
                }

                Log.Information("Scan complete.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error OCR Space");
            }

            return result;
        }
    }
}