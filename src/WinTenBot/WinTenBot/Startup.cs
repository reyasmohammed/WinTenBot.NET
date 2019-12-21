using System;
using Hangfire;
using Hangfire.LiteDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Extensions;
using WinTenBot.Handlers;
using WinTenBot.Handlers.Commands.Chat;
using WinTenBot.Handlers.Commands.Core;
using WinTenBot.Handlers.Commands.Group;
using WinTenBot.Handlers.Commands.Notes;
using WinTenBot.Handlers.Commands.Rules;
using WinTenBot.Handlers.Commands.Security;
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

            Bot.Client = new TelegramBotClient(Configuration["ZiziBetaBot:ApiToken"]);
//            Bot.Client.SendTextMessageAsync("-1001404591750", "Bot started");
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

            services.AddScoped<PingHandler>()
                .AddScoped<HelpCommand>();

            services.AddScoped<MediaReceivedHandler>();

            services.AddScoped<MigrateCommand>()
                .AddScoped<MediaFilterCommand>();

            services.AddScoped<CallTagsReceivedHandler>()
                .AddScoped<TagsCommand>()
                .AddScoped<TagCommand>()
                .AddScoped<UntagCommand>();

            services.AddScoped<NotesCommand>()
                .AddScoped<AddNotesCommand>();

            services.AddScoped<AdminCommand>()
                .AddScoped<PinCommand>()
                .AddScoped<ReportCommand>()
                .AddScoped<AfkCommand>();

            services.AddScoped<RulesCommand>();
            
            services.AddScoped<NewChatMembersEvent>()
                .AddScoped<LeftChatMemberEvent>()
                .AddScoped<PinnedMessageEvent>();

            services.AddScoped<WelcomeCommand>();
            services.AddScoped<SetWelcomeCommand>();

            services.AddScoped<PingCommand>()
                .AddScoped<DebugCommand>()
                .AddScoped<StartCommand>()
                .AddScoped<IdCommand>()
                .AddScoped<InfoCommand>();


//            services.AddDbContext<DataContext>(options => options.UseSqlite(@"Filename=./mydb.db;"));

//            var sqliteOptions = new SQLiteStorageOptions();
//            services.AddHangfire(configuration => configuration
//                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
//                .UseSimpleAssemblyNameTypeSerializer()
//                .UseRecommendedSerializerSettings()
//                //.UseMemoryStorage(new MemoryStorageOptions { JobExpirationCheckInterval = TimeSpan.FromMinutes(10) })
//                .UseSQLiteStorage("Filename=psm.db;", sqliteOptions)
//            );

            services.AddHangfireServer();
            services.AddHangfire(t => t.UseLiteDbStorage(Configuration[key: "CommonConfig:HangfireLiteDb"]));

//            services.AddHangfire(x =>
//                x.UseStorage(new MySqlStorage(Configuration["CommonConfig:ConnectionString"],
//                    new MySqlStorageOptions() {TablePrefix = "hangfire"}))
//            );
//
//            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseHangfireServer();
            app.UseHangfireDashboard();
            
//            DigestScheduler.SendMessage();

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
                    .UseWhen<PinnedMessageEvent>(When.NewPinnedMessage)
                    .UseWhen<MediaReceivedHandler>(When.MediaReceived)
                    .UseWhen(When.NewMessage, msgBranch => msgBranch
                        .UseWhen(When.NewTextMessage, txtBranch => txtBranch
                                .UseWhen<CallTagsReceivedHandler>(When.CallTagReceived)
                                .UseWhen<PingHandler>(When.PingReceived)
                                .UseWhen(When.NewCommand, cmdBranch => cmdBranch
                                    .UseCommand<AdminCommand>("admin")
                                    .UseCommand<AddNotesCommand>("addfilter")
                                    .UseCommand<AfkCommand>("afk")
                                    .UseCommand<DebugCommand>("dbg")
                                    .UseCommand<HelpCommand>("help")
                                    .UseCommand<IdCommand>("id")
                                    .UseCommand<InfoCommand>("info")
                                    .UseCommand<MediaFilterCommand>("mfil")
                                    .UseCommand<MigrateCommand>("migrate")
                                    .UseCommand<NotesCommand>("filters")
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