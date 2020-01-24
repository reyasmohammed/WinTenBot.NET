using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
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

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            var msg = context.Update.Message;
            _settingsService = new SettingsService(msg);

            if (msg.Chat.Type == ChatType.Private)
            {
                await _chatProcessor.SendAsync("Welcome hanya untuk grup saja");
                return;
            }

            var partsMsg = msg.Text.Split(' ').ToArray();

            string[] commands = {"message", "msg", "button", "btn"};
            ConsoleHelper.WriteLine(partsMsg.ToJson());

            var isAdmin = await _chatProcessor.IsAdminGroup();
            if (isAdmin)
            {
                if (msg.ReplyToMessage != null)
                {
                    var repMsg = msg.ReplyToMessage;
                    if (repMsg.GetFileId() != "")
                    {
                        var mediaType = repMsg.Type.ToString().ToLower();
                        await _chatProcessor.SendAsync("Sedang menyimpan Welcome Media..");
                        await _settingsService.UpdateCell("welcome_media", repMsg.GetFileId());
                        await _settingsService.UpdateCell("welcome_media_type", mediaType);
                        ConsoleHelper.WriteLine("Save media success..");

                        await _chatProcessor.EditAsync("Welcome Media berhasil di simpan.");
                        return;
                    }
                    else
                    {
                        await _chatProcessor.SendAsync("Media tidak terdeteksi di pesan yg di reply tersebut.");
                        return;
                    }
                }

                ConsoleHelper.WriteLine(partsMsg.Length);
                
                var missParamText = $"Parameter yg di dukung {string.Join(", ", commands)}" +
                               $"\nContoh: <code>/setwelcome message</code>";
                
                if (partsMsg.Length > 1)
                {
                    if (commands.Contains(partsMsg[1]))
                    {
                        if (partsMsg[2] != null)
                        {
                            var target = partsMsg[1]
                                .Replace("msg", "message")
                                .Replace("btn", "button");
                            var columnTarget = $"welcome_{target}";
                            var data = msg.Text
                                .Replace(partsMsg[0], "")
                                .Replace(partsMsg[1], "").Trim();

                            // ConsoleHelper.WriteLine(columnTarget);
                            // ConsoleHelper.WriteLine(data);

                            await _chatProcessor.SendAsync("Sedang menyimpan Welcome Message..");

                            await _settingsService.UpdateCell(columnTarget, data);
                            await _chatProcessor.EditAsync($"Welcome {target} berhasil di simpan!");
                        }
                        else
                        {
                            await _chatProcessor.SendAsync("Masukan Pesan atau tombol yang akan di tetapkan");
                        }
                    }
                    else
                    {
                       
                        await _chatProcessor.SendAsync(missParamText);
                    }
                }
                else
                {
                    await _chatProcessor.SendAsync(missParamText);
                }
            }
        }
    }
}