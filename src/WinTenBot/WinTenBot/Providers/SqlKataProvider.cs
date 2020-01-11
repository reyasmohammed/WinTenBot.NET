using MySql.Data.MySqlClient;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using WinTenBot.Helpers;
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
                Logger = result => { ConsoleHelper.WriteLine($"MySqlExec: {result}"); }
            };

            return factory;
        }

        public static Query ExecForMysql(this Query query)
        {
            var connection = new MySqlConnection(Bot.DbConnectionString);

            var factory = new QueryFactory(connection, new MySqlCompiler())
            {
                Logger = sql => { ConsoleHelper.WriteLine($"MySqlExec: {sql}"); }
            };

            return factory.FromQuery(query);
        }
    }
}