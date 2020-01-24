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
    }
}