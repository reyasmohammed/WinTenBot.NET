using System.Data.SQLite;
using System.IO;
using MySql.Data.MySqlClient;
using Serilog;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using WinTenBot.Model;

namespace WinTenBot.Providers
{
    public static class SqlKataProvider
    {
        public static QueryFactory GetMysqlInstances()
        {
            var connection = new MySqlConnection(Bot.DbConnectionString);

            var factory = new QueryFactory(connection, new MySqlCompiler())
            {
                Logger = result =>
                {
                    // ConsoleHelper.WriteLine($"MySqlExec: {result}"); 
                    Log.Debug($"MySqlExec: {result}");
                }
            };

            return factory;
        }

        public static Query ExecForMysql(this Query query, bool printSql = false)
        {
            var connection = new MySqlConnection(Bot.DbConnectionString);

            var factory = new QueryFactory(connection, new MySqlCompiler());
            if (printSql)
            {
                factory.Logger = sql =>
                {
                    Log.Debug($"MySqlExec: {sql}");
                    // ConsoleHelper.WriteLine($"MySqlExec: {sql}");
                };
            }

            return factory.FromQuery(query);
        }
    }
}