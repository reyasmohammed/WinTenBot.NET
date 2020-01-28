using Hangfire;
using Serilog;
using WinTenBot.Helpers;

namespace WinTenBot.Scheduler
{
    static class BotScheduler
    {
        public static void StartScheduler()
        {
            StartLogCleanupScheduler();
        }

        public static void StopScheduler()
        {
            
        }

        private static void StartLogCleanupScheduler()
        {
            var jobId = "cron-logfile-cleanup";

            Log.Debug($"Starting cron Log Cleaner with id {jobId}");

            RecurringJob.AddOrUpdate(jobId, () => BotHelper.ClearLog(), Cron.Hourly);
        }
    }
}
