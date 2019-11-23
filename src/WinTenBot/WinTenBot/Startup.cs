using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quickstart.AspNetCore.Handlers;
using Quickstart.AspNetCore.Options;
using Quickstart.AspNetCore.Services;
using System;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;

namespace Quickstart.AspNetCore
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<WinTenBot>()
                .Configure<BotOptions<WinTenBot>>(Configuration.GetSection("EchoBot"))
                .Configure<CustomBotOptions<WinTenBot>>(Configuration.GetSection("EchoBot"))
                .AddScoped<TextEchoer>()
                .AddScoped<PingCommand>()
                .AddScoped<StartCommand>()
                .AddScoped<WebhookLogger>()
                .AddScoped<StickerHandler>()
                .AddScoped<WeatherReporter>()
                .AddScoped<ExceptionHandler>()
                .AddScoped<UpdateMembersList>()
                .AddScoped<CallbackQueryHandler>()
                .AddScoped<IWeatherService, WeatherService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // get bot updates from Telegram via long-polling approach during development
                // this will disable Telegram webhooks
                app.UseTelegramBotLongPolling<WinTenBot>(ConfigureBot(), startAfter: TimeSpan.FromSeconds(2));
            }
            else
            {
                // use Telegram bot webhook middleware in higher environments
                app.UseTelegramBotWebhook<WinTenBot>(ConfigureBot());
                // and make sure webhook is enabled
                app.EnsureWebhookSet<WinTenBot>();
            }

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

        private IBotBuilder ConfigureBot()
        {
            return new BotBuilder()
                .Use<ExceptionHandler>()

                // .Use<CustomUpdateLogger>()
                .UseWhen<WebhookLogger>(When.Webhook)

                .UseWhen<UpdateMembersList>(When.MembersChanged)

                .UseWhen(When.NewMessage, msgBranch => msgBranch
                    .UseWhen(When.NewTextMessage, txtBranch => txtBranch
                        .Use<TextEchoer>()
                        .UseWhen(When.NewCommand, cmdBranch => cmdBranch
                            .UseCommand<PingCommand>("ping")
                            .UseCommand<StartCommand>("start")
                        )
                    //.Use<NLP>()
                    )
                    .UseWhen<StickerHandler>(When.StickerMessage)
                    .UseWhen<WeatherReporter>(When.LocationMessage)
                )

                .UseWhen<CallbackQueryHandler>(When.CallbackQuery)

                 //.Use<UnhandledUpdateReporter>()
                 ;
        }
    }
}