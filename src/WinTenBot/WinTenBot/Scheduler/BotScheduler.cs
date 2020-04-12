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
            StartLogglyCleanup();
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

        private static void StartLogglyCleanup()
        {
            const string jobId = "cron-loggly-cleanup";
            const string storageCaches = "Storage/Caches";
            
            Log.Debug($"Starting cron Loggly Cache Cleaner with id {jobId}");

            RecurringJob.AddOrUpdate(jobId, () => storageCaches.ClearLogs("loggly",false), Cron.Daily);
        }
    }
}
