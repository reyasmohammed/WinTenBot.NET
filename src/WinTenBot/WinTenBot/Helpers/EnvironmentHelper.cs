using Microsoft.Extensions.Hosting;
using WinTenBot.Model;

namespace WinTenBot.Helpers
{
    public static class EnvironmentHelper
    {
        public static bool IsDev()
        {
            return BotSettings.HostingEnvironment.IsDevelopment();
        }
    }
}