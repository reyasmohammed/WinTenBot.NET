using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Welcome
{
    public class SetWelcomeCommand : CommandBase
    {
        private ChatProcessor _chatProcessor;
        private SettingsService _settingsService;

        public SetWelcomeCommand()
        {
            _settingsService = new SettingsService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            var msg = context.Update.Message;

            if (msg.Chat.Type == ChatType.Private)
            {
                await _chatProcessor.SendAsync("Welcome hanya untuk grup saja");
                return;
            }

            var partsMsg = msg.Text.Split(' ').ToArray();

            string[] commands = {"message", "msg", "button", "btn", "media"};
            ConsoleHelper.WriteLine(partsMsg.ToJson());

            var isAdmin = await _chatProcessor.IsAdminGroup();
            if (isAdmin)
            {
                if (commands.Contains(partsMsg[1]))
                {
                    if (msg.ReplyToMessage != null)
                    {
                        var repMsg = msg.ReplyToMessage;
                        if (repMsg.GetFileId() != "")
                        {
                            var mediaType = repMsg.Type.ToString().ToLower();
                            await _chatProcessor.SendAsync("Saving Welcome Media..");
                            await _settingsService.UpdateCell(msg.Chat.Id, "welcome_media", repMsg.GetFileId());
                            await _settingsService.UpdateCell(msg.Chat.Id, "welcome_media_type", mediaType);
                        }

                        partsMsg = repMsg.Text.Split(' ').ToArray();
                    }

                    if (partsMsg[2] != "")
                    {
                        var target = partsMsg[1]
                            .Replace("msg", "message")
                            .Replace("btn", "button");
                        var columnTarget = $"welcome_{target}";
                        var data = msg.Text
                            .Replace(partsMsg[0], "")
                            .Replace(partsMsg[1], "").Trim();

                        ConsoleHelper.WriteLine(columnTarget);
                        ConsoleHelper.WriteLine(data);

                        await _chatProcessor.SendAsync("Saving Welcome Message..");

                        await _settingsService.UpdateCell(
                            msg.Chat.Id,
                            columnTarget,
                            data
                        );
                    }

                    await _chatProcessor.EditAsync("Saved!");
                }
                else
                {
                    var sendText = $"Parameter yg di dukung {string.Join(", ", commands)}" +
                                   $"\nContoh: <code>/setwelcome message</code>";
                    await _chatProcessor.SendAsync(sendText);
                }
            }
        }

        private async Task SaveWelcomeMessage()
        {
            await _chatProcessor.SendAsync("Saving Welcome Message..");
        }

        private async Task SaveWelcomeButton()
        {
            await _chatProcessor.SendAsync("Saving Welcome Button..");
        }
    }
}