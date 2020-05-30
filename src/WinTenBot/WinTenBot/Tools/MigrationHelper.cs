using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WinTenBot.Providers;

namespace WinTenBot.Tools
{
    [Obsolete("Soon replace with FluentMigration")]
    public static class MigrationHelper
    {
        public static async Task MigrateLocalStorage(this string tableName)
        {
            var filePath = Environment.CurrentDirectory + $"/Storage/SQL/Sqlite/{tableName}.sql";
            Log.Debug($"Migrating :{filePath}");
            await filePath.ExecuteFileForSqLite();
        }

        public static void MigrateMysql()
        {
            var path = Environment.CurrentDirectory + @"/Storage/SQL/MySql";
            var listFiles = System.IO.Directory.GetFiles(path).Where(f => f.EndsWith(".sql"));
            foreach (var file in listFiles)
            {
                Log.Information($"Migrating => {file}");
                var sql = File.ReadAllText(file);
                var result = sql.ExecForMysqlNonQuery(true);
                
                // Log.Information($"Result: {result}");
            }
        }

        public static void MigrateSqlite()
        {
            var path = Environment.CurrentDirectory + @"/Storage/SQL/Sqlite";
            var listFiles = System.IO.Directory.GetFiles(path);
            foreach (var file in listFiles)
            {
                Log.Information($"Migrating => {file}");
                var sql = File.ReadAllText(file);
                var result = sql.ExecForSqLite(true);

                // Log.Information($"Result: {result}");
            }
        }

        public static void MigrateAll(){
            MigrateMysql();
            MigrateSqlite();
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