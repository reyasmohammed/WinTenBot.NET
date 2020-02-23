using System;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace WinTenBot.Providers
{
    public class SerilogProvider
    {
        public static void InitializeSerilog()
        {
            var outputConsoleTemplate = "[{Timestamp:HH:mm:ss.ffff} {Level:u3}] {Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(theme: SystemConsoleTheme.Colored,
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    outputTemplate: outputConsoleTemplate)
                .WriteTo.File("Storage/Logs/Logs-.log",
                    rollingInterval: RollingInterval.Day,
                    flushToDiskInterval: TimeSpan.FromSeconds(1),
                    shared:true)
                .CreateLogger();

        }
    }
}