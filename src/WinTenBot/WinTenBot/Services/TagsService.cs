using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WinTenBot.Helpers;
using WinTenBot.Interfaces;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class TagsService : ITagsService
    {
        private MySqlProvider _mySqlProvider;

        public TagsService()
        {
            _mySqlProvider = new MySqlProvider();
        }

        public async Task<DataTable> GetTagsAsync()
        {
            var data = await _mySqlProvider.ExecQueryAsync("SELECT * FROM tags");
            return data;
        }

        public async Task<DataTable> GetTagsByGroupAsync(string column, long chatId)
        {
            var sql = $"SELECT {column} FROM tags WHERE id_chat = '{chatId}' ORDER BY tag";
            var data = await _mySqlProvider.ExecQueryAsync(sql);
            return data;
        }

        public async Task<DataTable> GetTagByTag(long chatId, string tag)
        {
            var sql = $"SELECT * FROM tags WHERE id_chat = '{chatId}' AND tag = '{tag}' ORDER BY tag";
            var data = await _mySqlProvider.ExecQueryAsync(sql);
            return data;
        }

        public async Task SaveTag(Dictionary<string, object> data)
        {
            var json = TextHelper.ToJson(data);
            ConsoleHelper.WriteLine(json);

            var insert = await _mySqlProvider.Insert("tags", data);
            ConsoleHelper.WriteLine($"SaveTag: {insert}");
        }
    }
}