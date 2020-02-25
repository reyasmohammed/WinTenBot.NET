using System.Threading.Tasks;
using Hangfire;
using Serilog;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Services;

namespace WinTenBot.Scheduler
{
    public static class RssScheduler
    {
        public static void InitScheduler()
        {
            Task.Run(async () =>
            {
                Log.Information("Initializing RSS Scheduler.");

                var baseId = "rss-scheduler";
                var cronInMinute = 5;
                var rssService = new RssService();
                
                Log.Information("Getting list Chat ID");
                var listChatId = await rssService.GetListChatIdAsync();
                foreach (RssSetting row in listChatId)
                {
                    var chatId = row.ChatId;
                    var recurringId = $"{chatId}-{baseId}";

                    Log.Information($"Creating Jobs for {chatId}");

                    RecurringJob.RemoveIfExists(recurringId);
                    RecurringJob.AddOrUpdate(recurringId, ()
                        => RssHelper.ExecBroadcasterAsync(chatId), $"*/{cronInMinute} * * * *");
                }

                Log.Information("Registering RSS Scheduler complete.");
            });
        }
    }
}