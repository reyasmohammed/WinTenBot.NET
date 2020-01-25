using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using WinTenBot.Helpers;

namespace WinTenBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(theme: SystemConsoleTheme.Colored,restrictedToMinimumLevel:LogEventLevel.Debug)
                .WriteTo.File("Storage/Logs/Logs-.txt",
                    rollingInterval: RollingInterval.Day,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .CreateLogger();

            Parallel.Invoke(
                async () => await "word_filter".MigrateLocalStorage(),
                async () => await "rss_history".MigrateLocalStorage());

            try
            {
                // BuildWebHost(args).Run();
                CreateWebHostBuilder(args).Build().Run();

                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostBuilder, configBuilder) => configBuilder
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{hostBuilder.HostingEnvironment.EnvironmentName}.json", true, true)
                    .AddJsonEnvVar("QUICKSTART_SETTINGS", true)
                ).UseStartup<Startup>();
        }
    }
}