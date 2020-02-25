using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Tags
{
    public class TagsCommand : CommandBase
    {
        private readonly TagsService _tagsService;
        private SettingsService _settingsService;
        private RequestProvider _requestProvider;

        public TagsCommand()
        {
            _tagsService = new TagsService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;
            _requestProvider = new RequestProvider(context);
            _settingsService = new SettingsService(msg);

            var id = msg.From.Id;
            var sendText = "Under maintenance";

            ConsoleHelper.WriteLine(id.IsSudoer());
            
            await _requestProvider.DeleteAsync(msg.MessageId);
            await _requestProvider.SendTextAsync("🔄 Loading tags..");
            var tagsData = await _tagsService.GetTagsByGroupAsync("*", msg.Chat.Id);
            var tagsStr = string.Empty;

            foreach (var tag in tagsData)
            {
                tagsStr += $"#{tag.Tag} ";
            }

            sendText = $"#️⃣<b> {tagsData.Count} Tags</b>\n" +
                       $"\n{tagsStr}";

            await _requestProvider.EditAsync(sendText);
            
            //            var jsonSettings = TextHelper.ToJson(currentSetting);
            //            ConsoleHelper.WriteLine($"CurrentSettings: {jsonSettings}");

            // var lastTagsMsgId = int.Parse(currentSetting.Rows[0]["last_tags_message_id"].ToString());

            var currentSetting = await _settingsService.GetSettingByGroup();
            var lastTagsMsgId = currentSetting.LastTagsMessageId;
            ConsoleHelper.WriteLine($"LastTagsMsgId: {lastTagsMsgId}");

            await _requestProvider.DeleteAsync(lastTagsMsgId.ToInt());

            await _tagsService.UpdateCacheAsync(msg);

            ConsoleHelper.WriteLine(_requestProvider.SentMessageId);
            await _settingsService.UpdateCell("last_tags_message_id", _requestProvider.SentMessageId);


//            var json = TextHelper.ToJson(tagsData);
            //                Console.WriteLine(json);
        
        }
    }
}