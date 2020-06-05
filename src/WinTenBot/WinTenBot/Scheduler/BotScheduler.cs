using sysIO = System.IO;
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
            // StartSyncGlobalBanToLocal();

            RssScheduler.InitScheduler();
        }

        public static void StopScheduler()
        {
        }

        private static void StartLogCleanupScheduler()
        {
            var jobId = "logfile-cleanup";
            var path = sysIO.Path.Combine("Storage", "Logs");

            Log.Debug($"Starting cron Log Cleaner with id {jobId}");
            RecurringJob.RemoveIfExists(jobId);
            RecurringJob.AddOrUpdate(jobId, () => path.ClearLogs("Zizi", true), Cron.Hourly);
            RecurringJob.Trigger(jobId);
        }

        private static void StartLogglyCleanup()
        {
            const string jobId = "loggly-cleanup";
            const string storageCaches = "Storage/Caches";

            Log.Debug($"Starting cron Loggly Cache Cleaner with id {jobId}");
            RecurringJob.RemoveIfExists(jobId);
            RecurringJob.AddOrUpdate(jobId, () => storageCaches.ClearLogs("Loggly", false), Cron.Hourly);
            RecurringJob.Trigger(jobId);
        }

        private static void StartSyncWordFilter()
        {
            const string jobId = "sync-word-filter";

            Log.Debug("Starting cron Sync Word Filter to Local Storage");
            RecurringJob.RemoveIfExists(jobId);
            RecurringJob.AddOrUpdate(jobId, () => Sync.SyncWordToLocalAsync(), Cron.Minutely);
            RecurringJob.Trigger(jobId);
        }
    }
}