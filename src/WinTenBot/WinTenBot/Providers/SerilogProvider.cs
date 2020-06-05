using System;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Datadog.Logs;
using Serilog.Sinks.SystemConsole.Themes;
using WinTenBot.Common;
using WinTenBot.Model;

namespace WinTenBot.Providers
{
    public static class SerilogProvider
    {
        public static string LogglyToken { get; set; }

        public static void InitializeSerilog()
        {
            const string outputTemplate = "[{Timestamp:HH:mm:ss.ffff} {Level:u3}] {Message:lj}{NewLine}{Exception}";
            var logPath = "Storage/Logs/ZiziBot-.log";
            var flushInterval = TimeSpan.FromSeconds(1);
            var rollingInterval = RollingInterval.Day;
            var datadogKey = BotSettings.DatadogApiKey;
            var rollingFile = 50 * 1024 * 1024;

            var serilogConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Debug)
                .MinimumLevel.Override("Hangfire", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: SystemConsoleTheme.Colored, outputTemplate: outputTemplate)
                .WriteTo.File(logPath, rollingInterval: rollingInterval, flushToDiskInterval: flushInterval,
                    shared: true, fileSizeLimitBytes: rollingFile);

            if (datadogKey != "YOUR_API_KEY" || datadogKey.IsNotNullOrEmpty())
            {
                var dataDogHost = "intake.logs.datadoghq.com";
                var config = new DatadogConfiguration(url: dataDogHost, port: 10516, useSSL: true, useTCP: true);
                serilogConfig.WriteTo.DatadogLogs(
                    apiKey: datadogKey,
                    service: "TelegramBot",
                    source: BotSettings.DatadogSource,
                    host: BotSettings.DatadogHost,
                    tags: BotSettings.DatadogTags.ToArray(),
                    configuration: config);
            }

            Log.Logger = serilogConfig.CreateLogger();

            // Log.Logger = new LoggerConfiguration()
            //     .MinimumLevel.Debug()
            //     .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
            //     .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Debug)
            //     .Enrich.FromLogContext()
            //     .WriteTo.Loggly(customerToken: LogglyToken, tags: logglyTags, bufferBaseFilename: logglyBuffer)
            //     .WriteTo.Console(theme: SystemConsoleTheme.Colored, outputTemplate: outputTemplate)
            //     .WriteTo.File(logPath, rollingInterval: rollingInterval, flushToDiskInterval: flushInterval,
            //         retainedFileCountLimit: 7)
            //     .CreateLogger();
        }
    }
}