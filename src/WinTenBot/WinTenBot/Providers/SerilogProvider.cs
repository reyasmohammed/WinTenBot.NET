using System;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace WinTenBot.Providers
{
    public static class SerilogProvider
    {
        public static void InitializeSerilog()
        {
            var outputConsoleTemplate = "[{Timestamp:HH:mm:ss.ffff} {Level:u3}] {Message:lj}{NewLine}{Exception}";
            var logPath = "Storage/Logs/Logs-.log";
            var flushInterval = TimeSpan.FromSeconds(1);
            var rollingInterval = RollingInterval.Day;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft",LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: SystemConsoleTheme.Colored, outputTemplate: outputConsoleTemplate)
                .WriteTo.File(logPath, rollingInterval: rollingInterval, flushToDiskInterval: flushInterval)
                .CreateLogger();
        }
    }
}