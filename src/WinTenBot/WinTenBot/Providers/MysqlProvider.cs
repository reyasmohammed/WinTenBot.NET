using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Serilog;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using WinTenBot.Model;

namespace WinTenBot.Providers
{
    public static class MysqlProvider
    {
        public static QueryFactory GetMysqlInstances()
        {
            var connection = new MySqlConnection(Bot.DbConnectionString);
            var factory = new QueryFactory(connection, new MySqlCompiler())
            {
                Logger = result => { Log.Debug($"MySqlExec: {result}"); }
            };
            return factory;
        }

        public static Query ExecForMysql(this Query query, bool printSql = false)
        {
            var connection = new MySqlConnection(Bot.DbConnectionString);
            var factory = new QueryFactory(connection, new MySqlCompiler());
            if (printSql)
            {
                factory.Logger = sql => { Log.Debug($"MySqlExec: {sql}"); };
            }
            return factory.FromQuery(query);
        }

        public static async Task<int> ExecForMysqlNonQueryAsync(this string sql, object param = null, bool printSql = false)
        {
            var connection = new MySqlConnection(Bot.DbConnectionString);
            var factory = new QueryFactory(connection, new MySqlCompiler());
            if (printSql)
            {
                factory.Logger = sqlResult => { Log.Debug($"MySqlExec: {sqlResult}"); };
            }
            return await factory.StatementAsync(sql, param);
        }
        
        public static int ExecForMysqlNonQuery(this string sql, object param = null, bool printSql = false)
        {
            var connection = new MySqlConnection(Bot.DbConnectionString);
            var factory = new QueryFactory(connection, new MySqlCompiler());
            if (printSql)
            {
                factory.Logger = sqlResult => { Log.Debug($"MySqlExec: {sqlResult}"); };
            }
            return factory.Statement(sql, param);
        }
        
        public static async Task<IEnumerable<dynamic>> ExecForMysqlQueryAsync(this string sql, object param = null, bool printSql = false)
        {
            var connection = new MySqlConnection(Bot.DbConnectionString);
            var factory = new QueryFactory(connection, new MySqlCompiler());
            if (printSql)
            {
                factory.Logger = sqlResult => { Log.Debug($"MySqlExec: {sqlResult}"); };
            }
            return await factory.SelectAsync(sql, param);
        }
        
        public static IEnumerable<dynamic> ExecForMysqlQuery(this string sql, object param = null, bool printSql = false)
        {
            var connection = new MySqlConnection(Bot.DbConnectionString);
            var factory = new QueryFactory(connection, new MySqlCompiler());
            if (printSql)
            {
                factory.Logger = sqlResult => { Log.Debug($"MySqlExec: {sqlResult}"); };
            }
            return factory.Select(sql, param);
        }
    }
}