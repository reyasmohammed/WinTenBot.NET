using Microsoft.Extensions.Options;
using Telegram.Bot.Framework;

namespace WinTenBot.Bots
{
    public class WinTenBot : BotBase
    {
        public WinTenBot(IOptions<BotOptions<WinTenBot>> options) : base(options.Value)
        {
        }
    }
}
