using Microsoft.Extensions.Configuration;

namespace WinTenBot.Model
{
    public class Bot
    {
        public static string DbConnectionString { get; set; }

        public static IConfiguration GlobalConfiguration { get; set; }
    }
}