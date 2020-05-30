using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Services;
using WinTenBot.Text;

namespace WinTenBot.Handlers.Commands.Tags
{
    public class TagsCommand : CommandBase
    {
        private readonly TagsService _tagsService;
        private SettingsService _settingsService;
        private TelegramService _telegramService;

        public TagsCommand()
        {
            _tagsService = new TagsService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;
            _telegramService = new TelegramService(context);
            _settingsService = new SettingsService(msg);

            var id = msg.From.Id;
            var sendText = "Under maintenance";

            await _telegramService.DeleteAsync(msg.MessageId);
            await _telegramService.SendTextAsync("🔄 Loading tags..");
            var tagsData = await _tagsService.GetTagsByGroupAsync("*", msg.Chat.Id);
            var tagsStr = string.Empty;

            foreach (var tag in tagsData)
            {
                tagsStr += $"#{tag.Tag} ";
            }

            sendText = $"#️⃣<b> {tagsData.Count} Tags</b>\n" +
                       $"\n{tagsStr}";

            await _telegramService.EditAsync(sendText);

            //            var jsonSettings = TextHelper.ToJson(currentSetting);
            //            Log.Information($"CurrentSettings: {jsonSettings}");

            // var lastTagsMsgId = int.Parse(currentSetting.Rows[0]["last_tags_message_id"].ToString());

            var currentSetting = await _settingsService.GetSettingByGroup();
            var lastTagsMsgId = currentSetting.LastTagsMessageId;
            Log.Information($"LastTagsMsgId: {lastTagsMsgId}");

            await _telegramService.DeleteAsync(lastTagsMsgId.ToInt());
            await _tagsService.UpdateCacheAsync(msg);
            await _settingsService.UpdateCell("last_tags_message_id", _telegramService.SentMessageId);


//            var json = TextHelper.ToJson(tagsData);
            //                Console.WriteLine(json);
        }
    }
}