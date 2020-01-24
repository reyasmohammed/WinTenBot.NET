using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Execution;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class NotesService
    {
        private readonly MySqlProvider _mySql;
        private string baseTable = "notes";

        public NotesService()
        {
            _mySql = new MySqlProvider();
        }

        public async Task<DataTable> GetNotesByChatId(long chatId)
        {
            var sql = $"SELECT * FROM {baseTable} WHERE chat_id = '{chatId}'";
            var data = await _mySql.ExecQueryAsync(sql);
            return data;
        }

        public async Task<List<CloudNote>> GetNotesBySlug(long chatId, string slug)
        {
            var query = await new Query(baseTable)
                .Where("chat_id",chatId)
                .OrWhereContains("slug",slug)
                .ExecForMysql()
                .GetAsync();

            var mapped = query.ToJson().MapObject<List<CloudNote>>();
            return mapped;
            // var sql = $"SELECT * FROM {baseTable} WHERE chat_id = '{chatId}' " +
            // $"AND MATCH(slug) AGAINST('{slug.SqlEscape()}')";
            // var data = await _mySql.ExecQueryAsync(sql);
            // return data;
        }

        public async Task SaveNote(Dictionary<string, object> data)
        {
            var json = data.ToJson();
            ConsoleHelper.WriteLine(json);

            var insert = await _mySql.Insert(baseTable, data);
            ConsoleHelper.WriteLine($"SaveNote: {insert}");
        }

        public async Task UpdateCache(long chatId)
        {
            var data = await GetNotesByChatId(chatId);
            await data.WriteCacheAsync($"{chatId}/notes.json");
        }
    }
}