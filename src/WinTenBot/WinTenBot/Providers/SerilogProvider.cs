using System;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace WinTenBot.Providers
{
    public static class SerilogProvider
    {
        public static string LogglyToken { get; set; }

        public static void InitializeSerilog()
        {
            const string outputTemplate = "[{Timestamp:HH:mm:ss.ffff} {Level:u3}] {Message:lj}{NewLine}{Exception}";
            var logPath = "Storage/Logs/ZiziBot-Logs-.log";
            var flushInterval = TimeSpan.FromSeconds(1);
            var rollingInterval = RollingInterval.Day;
            var logglyTags = "serilog,wintenbot";
            var logglyBuffer = "Storage/Caches/Loggly";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .WriteTo.Loggly(customerToken: LogglyToken, tags: logglyTags, bufferBaseFilename: logglyBuffer)
                .WriteTo.Console(theme: SystemConsoleTheme.Colored, outputTemplate: outputTemplate)
                .WriteTo.File(logPath, rollingInterval: rollingInterval, flushToDiskInterval: flushInterval,
                    retainedFileCountLimit: 7)
                .CreateLogger();
        }
    }
}