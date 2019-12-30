using Microsoft.Extensions.Options;
using Telegram.Bot.Framework;

namespace WinTenBot.Bots
{
    public class ZiziBot : BotBase
    {
        public ZiziBot(IOptions<BotOptions<ZiziBot>> options) : base(options.Value)
        {
        }
    }
}
