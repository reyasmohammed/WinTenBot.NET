using System.IO;
using System.Threading.Tasks;

namespace WinTenBot.Helpers
{
    public static class CoreHelper
    {
        public static async Task<string> LoadInBotDocs(this string slug)
        {
            var path = $"Storage/InbotDocs/{slug}.html";
            var html = await File.ReadAllTextAsync(path);

            return html.Trim();
        }
    }
}