using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Scheduler;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class RssCtlCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            var msg = context.Update.Message;

            var isSudoer = _telegramProvider.IsSudoer();
            if (isSudoer)
            {
                var partedMsg = msg.Text.Split(" ");
                var param1 = partedMsg.ValueOfIndex(1);
                Log.Debug($"RssCtl Param1: {param1}");

                await _telegramProvider.AppendTextAsync("Access Granted");
                switch (param1)
                {
                    case "start":
                        await _telegramProvider.AppendTextAsync("Starting RSS Service");
                        RssScheduler.InitScheduler();
                        await _telegramProvider.AppendTextAsync("Start successfully.");
                        break;

                    case "stop":
                        await _telegramProvider.AppendTextAsync("Stopping RSS Service");
                        HangfireHelper.DeleteAllJobs();
                        await _telegramProvider.AppendTextAsync("Stop successfully.");
                        break;
                }
            }
        }
    }
}