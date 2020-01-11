using Hangfire;
using Hangfire.Storage;

namespace WinTenBot.Helpers
{
    public class HangfireHelper
    {
        public static void DeleteAllJobs()
        {
            using (var connection = JobStorage.Current.GetConnection()) 
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    var recurringJobId = recurringJob.Id;
                    ConsoleHelper.WriteLine($"Deleting {recurringJobId}");
                    
                    RecurringJob.RemoveIfExists(recurringJobId);
                }
            }
        }
    }
}