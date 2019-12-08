using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using WinTenBot.Helpers;
using System.Linq;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class MediaFilterService
    {
        private MySqlProvider _mySqlProvider;
        private string baseTable = "media_filters";
        private string fileJson = "media_filter.json";


        public MediaFilterService()
        {
            _mySqlProvider = new MySqlProvider();
        }

        public async Task<bool> IsExist(string key, string value)
        {
            var sql = $"SELECT * FROM {baseTable} WHERE {key} = '{value}'";
            var data = await _mySqlProvider.ExecQueryAsync(sql);
            return data.Rows.Count > 0;
        }

        public async Task<bool> IsExistInCache(string key, string val)
        {
            var data = await ReadCacheAsync();
            var search = data.AsEnumerable()
                .Where(row => row.Field<string>(key) == val);
            if (!search.Any()) return false;

            var filtered = search.CopyToDataTable();
            ConsoleHelper.WriteLine($"Media found in Caches: {filtered.ToJson()}");
            return true;
        }

        public async Task SaveAsync(Dictionary<string, object> data)
        {
//            var json = TextHelper.ToJson(data);
            ConsoleHelper.WriteLine(data.ToJson());

            var insert = await _mySqlProvider.Insert(baseTable, data);
            ConsoleHelper.WriteLine($"SaveFile: {insert}");
        }

        public async Task<DataTable> GetAllMedia()
        {
            var sql = $"SELECT * FROM {baseTable}";
            var data = await _mySqlProvider.ExecQueryAsync(sql);
            return data;
        }

        public async Task UpdateCacheAsync()
        {
            var data = await GetAllMedia();
            ConsoleHelper.WriteLine($"Updating Media Filter caches to {fileJson}");

            await data.WriteCacheAsync(fileJson);
        }

        public async Task<DataTable> ReadCacheAsync()
        {
            var dataTable = await fileJson.ReadCacheAsync();
            return dataTable;
        }
    }
}