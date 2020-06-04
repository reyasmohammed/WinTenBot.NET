using System.Threading.Tasks;
using Hangfire;
using Serilog;
using WinTenBot.Model;
using WinTenBot.Services;
using WinTenBot.Telegram;
using WinTenBot.Text;
using WinTenBot.Tools;

namespace WinTenBot.Scheduler
{
    public static class RssScheduler
    {
        public static void InitScheduler()
        {
            Task.Run(async () =>
            {
                Log.Information("Initializing RSS Scheduler.");

                var baseId = "rss";
                var cronInMinute = 5;
                var rssService = new RssService();

                Log.Information("Getting list Chat ID");
                var listChatId = await rssService.GetListChatIdAsync()
                    .ConfigureAwait(false);
                foreach (RssSetting row in listChatId)
                {
                    var chatId = row.ChatId.ToInt64().ReduceChatId();
                    var recurringId = $"{baseId}-{chatId}";

                    Log.Information($"Creating Jobs for {chatId}");

                    RecurringJob.RemoveIfExists(recurringId);
                    RecurringJob.AddOrUpdate(recurringId, ()
                        => RssBroadcaster.ExecBroadcasterAsync(chatId), $"*/{cronInMinute} * * * *");
                }

                Log.Information("Registering RSS Scheduler complete.");
            });
        }

        public static void RegisterScheduler(this long chatId)
        {
            Log.Information("Initializing RSS Scheduler.");

            var baseId = "rss-scheduler";
            var cronInMinute = 5;
            var recurringId = $"{chatId}-{baseId}";

            Log.Information($"Creating Jobs for {chatId}");

            RecurringJob.RemoveIfExists(recurringId);
            RecurringJob.AddOrUpdate(recurringId, ()
                => RssBroadcaster.ExecBroadcasterAsync(chatId), $"*/{cronInMinute} * * * *");
            
            Log.Information("Registering RSS Scheduler complete.");
        }
    }
}