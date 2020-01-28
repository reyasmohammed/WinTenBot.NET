using System.Threading.Tasks;
using Serilog;
using WinTenBot.Providers;

namespace WinTenBot.Helpers
{
    public static class MigrationHelper
    {
        public static async Task MigrateLocalStorage(this string tableName)
        {
            var filePath = $"Storage/SQL/Sqlite/{tableName}.sql";
            Log.Debug($"Migrating :{filePath}");
            await filePath.ExecuteFileForSqLite();
        }

        public static void RunMigration()
        {
            Parallel.Invoke(
                async () => await "word_filter".MigrateLocalStorage(),
                async () => await "rss_history".MigrateLocalStorage(),
                async ()=> await "warn_username_history".MigrateLocalStorage());
        }
    }
}