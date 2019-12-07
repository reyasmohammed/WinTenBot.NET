using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WinTenBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilder, configBuilder) => configBuilder
                    .AddJsonFile("appsettings.json", true,  true)
                    .AddJsonFile($"appsettings.{hostBuilder.HostingEnvironment.EnvironmentName}.json", true, true)
                    .AddJsonEnvVar("QUICKSTART_SETTINGS", true)
                ).UseStartup<Startup>();
    }
}
