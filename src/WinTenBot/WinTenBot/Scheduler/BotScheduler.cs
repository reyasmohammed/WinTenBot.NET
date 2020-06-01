using Hangfire;
using Serilog;
using WinTenBot.Telegram;
using WinTenBot.Tools;

namespace WinTenBot.Scheduler
{
    static class BotScheduler
    {
        public static void StartScheduler()
        {
            StartLogCleanupScheduler();
            // StartLogglyCleanup();
            StartSyncWordFilter();
        }

        public static void StopScheduler()
        {
            
        }

        private static void StartLogCleanupScheduler()
        {
            var jobId = "cron-logfile-cleanup";

            Log.Debug($"Starting cron Log Cleaner with id {jobId}");

            RecurringJob.AddOrUpdate(jobId, () => Health.ClearLog(), Cron.Hourly);
            RecurringJob.Trigger(jobId);
        }

        private static void StartLogglyCleanup()
        {
            const string jobId = "cron-loggly-cleanup";
            const string storageCaches = "Storage/Caches";
            
            Log.Debug($"Starting cron Loggly Cache Cleaner with id {jobId}");

            RecurringJob.AddOrUpdate(jobId, () => storageCaches.ClearLogs("Loggly",false), Cron.Hourly);
            RecurringJob.Trigger(jobId);
        }

        private static void StartSyncWordFilter()
        {
            const string jobId = "cron-sync-word-filter";
            
            Log.Debug("Starting cron Sync Word Filter to Local Storage");
            RecurringJob.AddOrUpdate(jobId, () => Sync.SyncWordToLocalAsync(), Cron.Minutely);
            RecurringJob.Trigger(jobId);
        }
    }
}
