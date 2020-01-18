using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using WinTenBot.Helpers;
using WinTenBot.Model;

namespace WinTenBot.Providers
{
    [Obsolete("This class soon replaced by SQLKata.")]
    public class MySqlProvider
    {
        private MySqlConnection _mySqlConnection;
        private MySqlCommand _mySqlCommand;
        private MySqlDataAdapter _mySqlDataAdapter;
        //        private IConfiguration _config;

        public MySqlProvider()
        {
            _mySqlConnection = new MySqlConnection(Bot.DbConnectionString);
            _mySqlCommand = new MySqlCommand();
            _mySqlDataAdapter = new MySqlDataAdapter();
        }

        #region Connect Control

        private async Task<bool> OpenConnectionAsync()
        {
            bool isOpen = false;
            try
            {
                if (_mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                    isOpen = true;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine(ex.StackTrace);
            }

            return isOpen;
        }

        private async Task<bool> CloseConnectionAsync()
        {
            bool isClose = false;
            try
            {
                if (_mySqlConnection.State == ConnectionState.Open)
                {
                    await _mySqlConnection.CloseAsync();
                    isClose = true;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine(ex.StackTrace);
            }

            return isClose;
        }

        #endregion Connect Control

        #region DataControl

        public async Task<DataTable> ExecQueryAsync(string sql, MySqlParameterCollection parameters = null)
        {
            var dataTable = new DataTable();

            ConsoleHelper.WriteLine($"SQL: {sql}");
            await OpenConnectionAsync();
            _mySqlCommand.Connection = _mySqlConnection;
            _mySqlCommand.CommandText = sql;
            _mySqlDataAdapter.SelectCommand = _mySqlCommand;
            if (parameters != null)
            {
                foreach (MySqlParameter parameter in parameters)
                {
                    _mySqlCommand.Parameters.Add(parameter);
                }
                //                _mySqlCommand.Parameters.AddRange(parameters);
            }
            await _mySqlDataAdapter.FillAsync(dataTable);

            await CloseConnectionAsync();
            return dataTable;
        }

        public async Task<int> ExecNonQueryAsync(string sql)
        {
            var result = -1;

            ConsoleHelper.WriteLine($"SQL: {sql}");
            await OpenConnectionAsync();
            _mySqlCommand.Connection = _mySqlConnection;
            _mySqlCommand.CommandText = sql;
            result = await _mySqlCommand.ExecuteNonQueryAsync();

            await CloseConnectionAsync();
            return result;
        }

        #endregion DataControl

        public async Task<bool> Insert(string tabel, Dictionary<string, object> data)
        {
            var insert = false;
            var kolom = "";
            var value = "";
            var hitung = 1;

            foreach (var val in data)
            {
                if (hitung != data.Count)
                {
                    kolom += val.Key + ",";
                    value += $"'{val.Value}',";
                }
                else
                {
                    kolom += val.Key;
                    value += $"'{val.Value}'";
                }

                hitung++;
            }

            var query = $"INSERT INTO {tabel} ({kolom}) VALUES ({value})";
            var res = await ExecNonQueryAsync(query);
            if (res > 0) insert = true;

            return insert;
        }
    }
}