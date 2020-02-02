using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Telegram.Bot;

namespace WinTenBot.Model
{
    public class Bot
    {
        public static ITelegramBotClient Client { get; set; }

        public static Dictionary<string,ITelegramBotClient> Clients { get;set;} = new Dictionary<string, ITelegramBotClient>();

        public static string DbConnectionString { get; set; }

        public static IConfiguration GlobalConfiguration { get; set; }

        public static IWebHostEnvironment HostingEnvironment { get; set; }
    }
}