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
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;

            var isSudoer = _requestProvider.IsSudoer();
            if (isSudoer)
            {
                var partedMsg = msg.Text.Split(" ");
                var param1 = partedMsg.ValueOfIndex(1);
                Log.Debug($"RssCtl Param1: {param1}");

                await _requestProvider.AppendTextAsync("Access Granted");
                switch (param1)
                {
                    case "start":
                        await _requestProvider.AppendTextAsync("Starting RSS Service");
                        RssScheduler.InitScheduler();
                        await _requestProvider.AppendTextAsync("Start successfully.");
                        break;

                    case "stop":
                        await _requestProvider.AppendTextAsync("Stopping RSS Service");
                        HangfireHelper.DeleteAllJobs();
                        await _requestProvider.AppendTextAsync("Stop successfully.");
                        break;
                }
            }
        }
    }
}