using System;
using Hangfire.LiteDB;
using Hangfire.Storage.SQLite;
using Serilog;
using WinTenBot.Model; // using Hangfire.MySql;

namespace WinTenBot.Providers
{
    public static class HangfireProvider
    {
        // public static MySqlStorage GetMysqlStorage()
        // {
        //     var connectionString = BotSettings.DbConnectionString;
        //
        //     var options = new MySqlStorageOptions
        //     {
        //         TransactionIsolationLevel = IsolationLevel.ReadCommitted,
        //         QueuePollInterval = TimeSpan.FromSeconds(15),
        //         JobExpirationCheckInterval = TimeSpan.FromHours(1),
        //         CountersAggregateInterval = TimeSpan.FromMinutes(5),
        //         PrepareSchemaIfNecessary = true,
        //         DashboardJobListLimit = 50000,
        //         TransactionTimeout = TimeSpan.FromMinutes(1),
        //         TablesPrefix = "Hangfire_"
        //     };
        //     var storage = new MySqlStorage(connectionString, options);
        //     return storage;
        // }

        public static SQLiteStorage GetSqliteStorage()
        {
            var connectionString = BotSettings.GlobalConfiguration["Hangfire:Sqlite"];
            var options = new SQLiteStorageOptions()
            {
                QueuePollInterval = TimeSpan.FromSeconds(10)
            };

            var storage = new SQLiteStorage(connectionString, options);
            return storage;
        }

        public static LiteDbStorage GetLiteDbStorage()
        {
            var connectionString = BotSettings.GlobalConfiguration["Hangfire:LiteDb"];
            var options = new LiteDbStorageOptions()
            {
                QueuePollInterval = TimeSpan.FromSeconds(10)
            };

            var storage = new LiteDbStorage(connectionString, options);
            return storage;
        }
    }
}