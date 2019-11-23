using Microsoft.Extensions.Options;
using Telegram.Bot.Framework;

namespace Quickstart.AspNetCore
{
    public class WinTenBot : BotBase
    {
        public WinTenBot(IOptions<BotOptions<WinTenBot>> options)
            : base(options.Value)
        {
        }
    }
}
