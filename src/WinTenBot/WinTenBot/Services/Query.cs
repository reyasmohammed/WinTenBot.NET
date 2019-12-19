using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class Query
    {
        private readonly MySqlProvider _dbConn = new MySqlProvider();

        public async Task<bool> IsTableExist(string tabel)
        {
            var cek = false;

            var query = "SELECT * FROM " + tabel;
            var dTbl = await _dbConn.ExecQueryAsync(query);

            if (dTbl.Rows.Count > 0) cek = true;

            return cek;
        }

        public async Task<bool> IsDataExist(string tabel, Dictionary<string, object> where)
        {
            var query = $"SELECT * FROM {tabel} WHERE {where.First().Key} = \'{where.First().Value}\'";
            var dTbl = await _dbConn.ExecQueryAsync(query);

            if (dTbl.Rows.Count > 0)
                return true;
            return false;
        }

        public async Task<string> BuatId(string tabel, string id, string prefix)
        {
            var kode = "";
            var idx = 0;
            var query = "SELECT IFNULL(MAX(SUBSTRING(" + id + ", 3, 4)), 0) as " + id + " from " + tabel;
            var dtTbl = await _dbConn.ExecQueryAsync(query);

            if (dtTbl.Rows.Count > 0)
                foreach (DataRow tmp in dtTbl.Rows)
                    idx = Convert.ToInt32(tmp[id].ToString());

            if (idx >= 0 && idx <= 8)
                kode = prefix + "00" + Convert.ToInt32(idx + 1);
            else if (idx >= 9 && idx <= 98)
                kode = prefix + "0" + Convert.ToInt32(idx + 1);
            else if (idx >= 99 && idx <= 998) kode = prefix + Convert.ToInt32(idx + 1);

            return kode;
        }

        public async Task<DataTable> Select(string tabel, string cari)
        {
            var query1 = "SELECT GROUP_CONCAT(column_name ORDER BY ordinal_position) as concats" +
                         " FROM information_schema.columns" +
                         " WHERE table_schema = DATABASE() and table_name = '" + tabel + "'" +
                         " GROUP BY table_name ORDER BY table_name";
            var dTbl1 = await _dbConn.ExecQueryAsync(query1);
            if (dTbl1.Rows.Count > 0)
            {
                var concats = dTbl1.Rows[0];

                var query2 = "SELECT * FROM " + tabel + " WHERE concat(" + concats["concats"] + ") like '%" + cari + "%'";
                var dTbl2 = await _dbConn.ExecQueryAsync(query2);

                if (dTbl2.Rows.Count > 0)
                    return dTbl2;
            }
            return new DataTable(null);
        }

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
                    value += "'" + val.Value.ToString().SqlEscape() + "',";
                }
                else
                {
                    kolom += val.Key;
                    value += "'" + val.Value.ToString().SqlEscape() + "'";
                }

                hitung++;
            }

            var query = "INSERT INTO " + tabel + " (" + kolom + ") VALUES (" + value + ")";
            var result = await _dbConn.ExecNonQueryAsync(query);
            if (result > 0) insert = true;

            return insert;
        }

        public async Task<bool> Update(string tabel, Dictionary<string, object> data, Dictionary<string, object> where)
        {
            var update = false;
            var kolom = "";
            var hitung = 1;

            foreach (var val in data)
            {
                if (hitung != data.Count)
                    kolom += val.Key + " = '" + val.Value.ToString().SqlEscape() + "', ";
                else
                    kolom += val.Key + " = '" + val.Value.ToString().SqlEscape() + "' ";
                hitung++;
            }

            var query = $"UPDATE {tabel} SET {kolom} WHERE {where.First().Key} = \'{where.First().Value}\'";
            var result = await _dbConn.ExecNonQueryAsync(query);
            if ( result> 0) update = true;

            return update;
        }

        public async Task<bool> Delete(string tabel, Dictionary<string, object> where)
        {
            var hapus = false;

            var query = $"DELETE FROM {tabel} WHERE {where.First().Key} = \'{where.First().Value}\'";
            var result = await _dbConn.ExecNonQueryAsync(query);
            if ( result> 0) hapus = true;

            return hapus;
        }

        public async Task<DataTable> GetQuery(string query)
        {
            var dTbl = await _dbConn.ExecQueryAsync(query);

            if (dTbl.Rows.Count > 0)
                return dTbl;
            return new DataTable(null);
        }

        public async Task<bool> ExeQuery(string query)
        {
            var result = await _dbConn.ExecNonQueryAsync(query);
            if ( result> 0) return true;
            return false;
        }
        
    }
}
