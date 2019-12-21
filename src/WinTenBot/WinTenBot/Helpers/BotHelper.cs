using System.Threading.Tasks;
using WinTenBot.Model;

namespace WinTenBot.Helpers
{
    public static class BotHelper
    {
        public static async Task<string> GetUrlStart(this string param)
        {
            var bot = await Bot.Client.GetMeAsync();
            var username = bot.Username;
            return $"https://t.me/{username}?start={param}";
        }
    }
}