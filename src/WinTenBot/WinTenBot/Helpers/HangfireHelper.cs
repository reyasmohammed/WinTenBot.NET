using Hangfire;
using Hangfire.Storage;
using Serilog;

namespace WinTenBot.Helpers
{
    public static class HangfireHelper
    {
        public static void DeleteAllJobs()
        {
            using var connection = JobStorage.Current.GetConnection();
            foreach (var recurringJob in connection.GetRecurringJobs())
            {
                var recurringJobId = recurringJob.Id;
                Log.Information($"Deleting {recurringJobId}");
                    
                RecurringJob.RemoveIfExists(recurringJobId);
            }
        }
    }
}