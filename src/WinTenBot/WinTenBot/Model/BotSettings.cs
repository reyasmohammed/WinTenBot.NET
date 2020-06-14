﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using WinTenBot.Common;
using WinTenBot.IO;

namespace WinTenBot.Model
{
    public static class BotSettings
    {
        public static void FillSettings()
        {
            ProductName = GlobalConfiguration["Engines:ProductName"];
            ProductVersion = GlobalConfiguration["Engines:Version"];
            ProductCompany = GlobalConfiguration["Engines:Company"];

            Sudoers = GlobalConfiguration.GetSection("Sudoers").Get<List<string>>();
            BotChannelLogs = GlobalConfiguration["CommonConfig:ChannelLogs"].ToInt64();
            SpamWatchToken = GlobalConfiguration["CommonConfig:SpamWatchToken"];

            DbConnectionString = GlobalConfiguration["CommonConfig:ConnectionString"];

            GoogleCloudCredentialsPath = GlobalConfiguration["GoogleCloud:CredentialsPath"];
            
            HangfireMysqlDb = GlobalConfiguration["Hangfire:MySql"];
            HangfireSqliteDb = GlobalConfiguration["Hangfire:Sqlite"];
            HangfireLiteDb = GlobalConfiguration["Hangfire:LiteDb"];

            SerilogLogglyToken = GlobalConfiguration["CommonConfig:LogglyToken"];

            DatadogApiKey = GlobalConfiguration["Datadog:ApiKey"];
            DatadogHost = GlobalConfiguration["Datadog:Host"];
            DatadogSource = GlobalConfiguration["Datadog:Source"];
            DatadogTags = GlobalConfiguration.GetSection("Datadog:Tags").Get<List<string>>();

            IbmWatsonTranslateUrl = GlobalConfiguration["IbmConfig:Watson:TranslateUrl"];
            IbmWatsonTranslateToken = GlobalConfiguration["IbmConfig:Watson:TranslateToken"];

            LearningDataSetPath = @"Storage\Learning\".EnsureDirectory();
            TesseractTrainedData = @"Storage\Data\Tesseract\";
            PathCache = "Storage/Caches";

            OcrSpaceKey = GlobalConfiguration["OcrSpace:ApiKey"];
        }

        public static string ProductName { get; set; }
        public static string ProductVersion { get; set; }
        public static string ProductCompany { get; set; }
        public static ITelegramBotClient Client { get; set; }

        public static Dictionary<string, ITelegramBotClient> Clients { get; set; } =
            new Dictionary<string, ITelegramBotClient>();

        public static string DbConnectionString { get; set; }

        public static IConfiguration GlobalConfiguration { get; set; }

        public static IWebHostEnvironment HostingEnvironment { get; set; }
        public static bool IsDevelopment => HostingEnvironment.IsDevelopment();
        public static bool IsStaging => HostingEnvironment.IsStaging();
        public static bool IsProduction => HostingEnvironment.IsProduction();

        public static bool IsEnvironment(string envName)
        {
            return HostingEnvironment.IsEnvironment(envName);
        }

        public static string PathCache { get; set; }
        public static string LearningDataSetPath { get; set; }

        public static List<string> Sudoers { get; set; }
        public static long BotChannelLogs { get; set; }
        public static string SpamWatchToken { get; set; }

        public static string GoogleCloudCredentialsPath { get; set; }

        public static string HangfireMysqlDb { get; set; }
        public static string HangfireSqliteDb { get; set; }
        public static string HangfireLiteDb { get; set; }

        public static string SerilogLogglyToken { get; set; }

        public static string DatadogApiKey { get; set; }
        public static string DatadogHost { get; set; }
        public static string DatadogSource { get; set; }
        public static List<string> DatadogTags { get; set; }

        public static string IbmWatsonTranslateUrl { get; set; }
        public static string IbmWatsonTranslateToken { get; set; }

        public static string TesseractTrainedData { get; set; }

        public static string OcrSpaceKey { get; set; }
    }
}