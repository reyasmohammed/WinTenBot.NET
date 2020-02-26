using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Providers;

namespace WinTenBot.Services
{
    public class SettingsService
    {
        private string baseTable = "group_settings";
        private Message Message { get; set; }

        public SettingsService(Message message)
        {
            // Chat = chat;
            Message = message;
        }

        public async Task<bool> IsSettingExist()
        {
            var where = new Dictionary<string, object>() {{"chat_id", Message.Chat.Id}};

            var data = await new Query(baseTable)
                .Where(where)
                .ExecForMysql()
                .GetAsync();
            var isExist = data.Any();

            Log.Information($"Group setting IsExist: {isExist}");
            return isExist;
        }

        public async Task<ChatSetting> GetSettingByGroup()
        {
            var where = new Dictionary<string, object>()
            {
                {"chat_id", Message.Chat.Id}
            };

            var data = await new Query(baseTable)
                .Where(where)
                .ExecForMysql()
                .GetAsync();

            var mapped = data.ToJson().MapObject<List<ChatSetting>>();

            return mapped.FirstOrDefault();
        }

        public async Task<List<CallBackButton>> GetSettingButtonByGroup()
        {
            var where = new Dictionary<string, object>()
            {
                {"chat_id", Message.Chat.Id}
            };

            var selectColumns = new[]
            {
                "id", "enable_word_filter_per_group", "enable_word_filter_group_wide","enable_warn_username",
                "enable_welcome_message","enable_welcome_message","enable_badword_filter","enable_anti_malfiles"
            };

            var data = await new Query(baseTable)
                .Select(selectColumns)
                .Where(where)
                .ExecForMysql()
                .GetAsync();

            // Log.Debug($"PreTranspose: {data.ToJson()}");
            // data.ToJson(true).ToFile("settings_premap.json");
            
            var dataTable = data.ToJson().ToDataTable();
            
            var rowId = dataTable.Rows[0]["id"].ToString();
            Log.Debug($"RowId: {rowId}");

            var transposedTable = dataTable.TransposedTable();
            // Log.Debug($"PostTranspose: {transposedTable.ToJson()}");
            // transposedTable.ToJson(true).ToFile("settings_premap.json");

            // Log.Debug("Setting Buttons:{0}", transposedTable.ToJson());

            var listBtn = new List<CallBackButton>();
            foreach (DataRow row in transposedTable.Rows)
            {
                var textOrig = row["id"].ToString();
                var value = row[rowId].ToString();

                var boolVal = value.ToBool();

                var forCallbackData = textOrig;
                var btnText = textOrig
                    .Replace("enable_", "")
                    .Replace("_"," ");
                
                if (boolVal)
                {
                    forCallbackData = textOrig.Replace("enable", "disable");
                }

                // listBtn.Add(new CallBackButton()
                // {
                //     Text = row["id"].ToString(),
                //     Data = row[rowId].ToString()
                // });
                
                listBtn.Add(new CallBackButton()
                {
                    Text = btnText.ToTitleCase(),
                    Data = $"setting {forCallbackData}"
                });
            }


            //
            // listBtn.Add(new CallBackButton()
            // {
            //     Text = "Enable Word filter Per-Group",
            //     Data = $"setting {mapped.EnableWordFilterGroupWide.ToString()}_word_filter_per_group"
            // });

            // var x =mapped.Cast<CallBackButton>();

            // MatrixHelper.TransposeMatrix<List<ChatSetting>(mapped);
            Log.Debug($"ListBtn: {listBtn.ToJson()}");
            listBtn.ToJson(true).ToFile("settings_listbtn.json");

            return listBtn;
        }

        public async Task<int> SaveSettingsAsync(Dictionary<string, object> data)
        {
            var where = new Dictionary<string, object>() {{"chat_id", data["chat_id"]}};

            var check = await new Query(baseTable)
                .Where(where)
                .ExecForMysql()
                .GetAsync();
            var isExist = check.Any();
            
            var insert = -1;
            Log.Information($"Group setting IsExist: {isExist}");
            if (!isExist)
            {
                Log.Information($"Inserting new data for {Message.Chat}");

                insert = await new Query(baseTable)
                    .ExecForMysql()
                    .InsertAsync(data);
            }
            else
            {
                Log.Information($"Updating data for {Message.Chat}");

                insert = await new Query(baseTable)
                    .Where(where)
                    .ExecForMysql()
                    .UpdateAsync(data);
            }

            return insert;
        }

        public async Task UpdateCell(string key, object value)
        {
            var where = new Dictionary<string, object>() {{"chat_id", Message.Chat.Id}};
            var data = new Dictionary<string, object>() {{key, value}};

            await new Query(baseTable)
                .Where(where)
                .ExecForMysql()
                .UpdateAsync(data);
        }

        public async Task UpdateCache()
        {
            var data = await GetSettingByGroup();
            await data.WriteCacheAsync($"{Message.Chat.Id}/settings.json");
        }
    }
}