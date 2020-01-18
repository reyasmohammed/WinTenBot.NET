using System.Data.SQLite;
using System.IO;
using Serilog;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace WinTenBot.Providers
{
    public static class LocalStorageProvider
    {
        static string dbPath = "Storage/Common/LocalStorage.db";

        private static SQLiteConnection InitSqLite()
        {
            if (File.Exists(dbPath)) return new SQLiteConnection($"Data Source={dbPath};Version=3;");
            
            Log.Information($"Creating {dbPath} for LocalStorage");
            SQLiteConnection.CreateFile(dbPath);

            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }
        
        public static Query ExecForSqLite(this Query query, bool printSql = false)
        {
            // var connection = new SQLiteConnection(dbPath);
            var connection = InitSqLite();
            
            var factory = new QueryFactory(connection, new SqliteCompiler());

            if (printSql) factory.Logger = sqlResult => { Log.Debug($"SQLiteExec: {sqlResult}"); };

            return factory.FromQuery(query);
        }
    }
}