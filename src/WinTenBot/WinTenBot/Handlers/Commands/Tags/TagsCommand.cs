using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Tags
{
    public class TagsCommand : CommandBase
    {
        private readonly TagsService _tagsService;
        private readonly SettingsService _settingsService;
        private ChatProcessor _chatProcessor;

        public TagsCommand()
        {
            _tagsService = new TagsService();
            _settingsService = new SettingsService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);

            Message msg = context.Update.Message;

            
            var id = msg.From.Id;
            var sendText = "Under maintenance";

            ConsoleHelper.WriteLine(id.IsSudoer());
            
            await _chatProcessor.DeleteAsync(msg.MessageId);
            await _chatProcessor.SendAsync("🔄 Loading tags..");
            var tagsData = await _tagsService.GetTagsByGroupAsync("*", msg.Chat.Id);
            var tagsStr = string.Empty;

            foreach (DataRow tag in tagsData.Rows)
            {
                tagsStr += $"#{tag["tag"]} ";
            }

            sendText = $"#️⃣<b>{tagsData.Rows.Count} Tags</b>\n" +
                       $"\n{tagsStr}";

            await _chatProcessor.EditAsync(sendText);

            var currentSetting = await _settingsService.GetSettingByGroup(msg.Chat.Id);
//            var jsonSettings = TextHelper.ToJson(currentSetting);
//            ConsoleHelper.WriteLine($"CurrentSettings: {jsonSettings}");
            
            var lastTagsMsgId = int.Parse(currentSetting.Rows[0]["last_tags_message_id"].ToString());
            ConsoleHelper.WriteLine($"LastTagsMsgId: {lastTagsMsgId}");

            await _chatProcessor.DeleteAsync(lastTagsMsgId);

            ConsoleHelper.WriteLine(_chatProcessor.SentMessageId);
            await _settingsService.UpdateCell(msg.Chat.Id, "last_tags_message_id", _chatProcessor.SentMessageId);


//            var json = TextHelper.ToJson(tagsData);
            //                Console.WriteLine(json);
        
        }
    }
}