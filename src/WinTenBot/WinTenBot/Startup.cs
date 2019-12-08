using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Extensions;
using WinTenBot.Handlers;
using WinTenBot.Handlers.Commands;
using WinTenBot.Handlers.Commands.Core;
using WinTenBot.Handlers.Commands.Group;
using WinTenBot.Handlers.Commands.Rules;
using WinTenBot.Handlers.Commands.Tags;
using WinTenBot.Handlers.Commands.Welcome;
using WinTenBot.Handlers.Events;
using WinTenBot.Interfaces;
using WinTenBot.Model;
using WinTenBot.Options;
using WinTenBot.Services;

namespace WinTenBot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Bot.GlobalConfiguration = Configuration;
            Bot.DbConnectionString = Configuration["CommonConfig:ConnectionString"];

            Console.WriteLine($"ProductName: {Configuration["Engines:ProductName"]}");
            Console.WriteLine($"Version: {Configuration["Engines:Version"]}");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<WinTenBot>()
                .Configure<BotOptions<WinTenBot>>(Configuration.GetSection("ZiziBetaBot"))
                .Configure<CustomBotOptions<WinTenBot>>(Configuration.GetSection("ZiziBetaBot"))
                .AddScoped<GenericMessageHandler>()
                .AddScoped<WebhookLogger>()
                .AddScoped<StickerHandler>()
                .AddScoped<WeatherReporter>()
                .AddScoped<ExceptionHandler>()
                .AddScoped<UpdateMembersList>()
                .AddScoped<CallbackQueryHandler>()
                .AddScoped<IWeatherService, WeatherService>();

            services.AddScoped<MediaReceivedHandler>();

            services.AddScoped<MigrateCommand>()
                .AddScoped<MediaFilterCommand>();

            services.AddScoped<CallTagsReceivedHandler>()
                .AddScoped<TagsCommand>()
                .AddScoped<TagCommand>()
                .AddScoped<UntagCommand>();


            services.AddScoped<AdminCommand>();
            services.AddScoped<NewChatMembersEvent>();
            services.AddScoped<LeftChatMemberEvent>();

            services.AddScoped<PinCommand>();

            services.AddScoped<RulesCommand>()
                .AddScoped<ReportCommand>();

            services.AddScoped<WelcomeCommand>();
            services.AddScoped<SetWelcomeCommand>();

            services.AddScoped<PingCommand>()
                .AddScoped<DebugCommand>()
                .AddScoped<StartCommand>()
                .AddScoped<IdCommand>()
                .AddScoped<InfoCommand>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Bot.HostingEnvironment = env;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // get bot updates from Telegram via long-polling approach during development
                // this will disable Telegram webhooks
                app.UseTelegramBotLongPolling<WinTenBot>(ConfigureBot(), startAfter: TimeSpan.FromSeconds(1));
            }
            else
            {
                // use Telegram bot webhook middleware in higher environments
                app.UseTelegramBotWebhook<WinTenBot>(ConfigureBot());
                // and make sure webhook is enabled
                app.EnsureWebhookSet<WinTenBot>();
            }

            app.Run(async context => { await context.Response.WriteAsync("Hello World!"); });
        }

        private IBotBuilder ConfigureBot()
        {
            return new BotBuilder()
                    .Use<ExceptionHandler>()
                    // .Use<CustomUpdateLogger>()
                    .UseWhen<WebhookLogger>(When.Webhook)

                    //.UseWhen<UpdateMembersList>(When.MembersChanged)
                    .UseWhen<NewChatMembersEvent>(When.NewChatMembers)
                    .UseWhen<LeftChatMemberEvent>(When.LeftChatMember)

                    //.UseWhen(When.MembersChanged, memberChanged => memberChanged
                    //    .UseWhen(When.MembersChanged, cmdBranch => cmdBranch
                    //        .Use<NewChatMembersCommand>()
                    //        )
                    //    )
                    .UseWhen<MediaReceivedHandler>(When.MediaReceived)
                    .UseWhen(When.NewMessage, msgBranch => msgBranch
                        .UseWhen(When.NewTextMessage, txtBranch => txtBranch
                                .UseWhen<CallTagsReceivedHandler>(When.CallTagRecieved)
                                .UseWhen(When.NewCommand, cmdBranch => cmdBranch
                                    .UseCommand<AdminCommand>("admin")
                                    .UseCommand<DebugCommand>("dbg")
                                    .UseCommand<IdCommand>("id")
                                    .UseCommand<InfoCommand>("info")
                                    .UseCommand<MediaFilterCommand>("mfil")
                                    .UseCommand<MigrateCommand>("migrate")
                                    .UseCommand<PinCommand>("pin")
                                    .UseCommand<PingCommand>("ping")
                                    .UseCommand<ReportCommand>("report")
                                    .UseCommand<RulesCommand>("rules")
                                    .UseCommand<SetWelcomeCommand>("setwelcome")
                                    .UseCommand<StartCommand>("start")
                                    .UseCommand<TagCommand>("tag")
                                    .UseCommand<TagsCommand>("tags")
                                    .UseCommand<UntagCommand>("untag")
                                    .UseCommand<WelcomeCommand>("welcome")
                                )
                                .Use<GenericMessageHandler>()

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