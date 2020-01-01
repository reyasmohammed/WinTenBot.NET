using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
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

        public static async Task<bool> IsBotAdded(this User[] users)
        {
            var me = await Bot.Client.GetMeAsync();
            return (from user in users where user.Id == me.Id select user.Id == me.Id).FirstOrDefault();
        }
    }
}