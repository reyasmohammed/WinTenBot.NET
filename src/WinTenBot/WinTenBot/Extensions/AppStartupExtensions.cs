using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Options;
using WinTenBot.Providers;

namespace WinTenBot.Extensions
{
    internal static class AppStartupExtensions
    {
        public static IApplicationBuilder UseTelegramBotLongPolling<TBot>(
            this IApplicationBuilder app,
            IBotBuilder botBuilder,
            TimeSpan startAfter = default,
            CancellationToken cancellationToken = default
        )
            where TBot : BotBase
        {
            if (startAfter == default)
            {
                startAfter = TimeSpan.FromSeconds(2);
            }

            var updateManager = new UpdatePollingManager<TBot>(botBuilder, new BotServiceProvider(app));

            Task.Run(async () =>
                {
                    await Task.Delay(startAfter, cancellationToken);
                    await updateManager.RunAsync(cancellationToken: cancellationToken);
                }, cancellationToken)
                .ContinueWith(t =>
                {
                    // ToDo use logger
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(t.Exception);
                    Console.ResetColor();
                    throw t.Exception;
                }, TaskContinuationOptions.OnlyOnFaulted);

            return app;
        }

        public static IApplicationBuilder EnsureWebhookSet<TBot>(
            this IApplicationBuilder app
        )
            where TBot : IBot
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                var bot = scope.ServiceProvider.GetRequiredService<TBot>();
                var options = scope.ServiceProvider.GetRequiredService<IOptions<CustomBotOptions<TBot>>>();
                var botToken = options.Value.ApiToken;
                var webhookPath = options.Value.WebhookPath;

                var url = new Uri(new Uri(options.Value.WebhookDomain), $"{webhookPath}/{botToken}/webhook");
                Log.Information($"Url WebHook: {url}");

                logger.LogInformation($"Setting WebHook for bot {typeof(TBot).Name} to URL {url}");

                bot.Client.SetWebhookAsync(url.AbsoluteUri)
                    .GetAwaiter().GetResult();
            }

            return app;
        }
    }
}