using Microsoft.Extensions.Options;
using Telegram.Bot.Framework;

namespace WinTenBot
{
    public class WinTenBot : BotBase
    {
        public WinTenBot(IOptions<BotOptions<WinTenBot>> options)
            : base(options.Value)
        {
        }
    }
}
