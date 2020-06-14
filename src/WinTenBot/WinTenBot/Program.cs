using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace WinTenBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Information("Starting WebAPI..");
                // BuildWebHost(args).Run();
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
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
                    .AddJsonFile("Storage/Config/security-base.json", true, true)
                    .AddJsonEnvVar("QUICKSTART_SETTINGS", true)
                ).UseStartup<Startup>()
                .UseSerilog();
        }
    }
}