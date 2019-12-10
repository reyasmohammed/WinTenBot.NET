using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;

namespace WinTenBot.Model
{
    public class Bot
    {
        public static ITelegramBotClient Client { get; set; }
        public static string DbConnectionString { get; set; }

        public static IConfiguration GlobalConfiguration { get; set; }

        public static IHostingEnvironment HostingEnvironment { get; set; }
    }
}