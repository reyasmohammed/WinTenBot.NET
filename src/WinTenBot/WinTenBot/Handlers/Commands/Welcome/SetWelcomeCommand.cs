using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Welcome
{
    public class SetWelcomeCommand : CommandBase
    {
        private SettingsService _settingsService;
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            var msg = context.Update.Message;
            _settingsService = new SettingsService(msg);

            if (msg.Chat.Type == ChatType.Private)
            {
                await _telegramProvider.SendTextAsync("Welcome hanya untuk grup saja");
                return;
            }

            var partsMsg = msg.Text.Split(' ').ToArray();

            string[] commands = {"message", "msg", "button", "btn"};
            Log.Information(partsMsg.ToJson());

            var isAdmin = await _telegramProvider.IsAdminGroup();
            if (isAdmin)
            {
                if (msg.ReplyToMessage != null)
                {
                    var repMsg = msg.ReplyToMessage;
                    if (repMsg.GetFileId() != "")
                    {
                        var mediaType = repMsg.Type.ToString().ToLower();
                        await _telegramProvider.SendTextAsync("Sedang menyimpan Welcome Media..");
                        await _settingsService.UpdateCell("welcome_media", repMsg.GetFileId());
                        await _settingsService.UpdateCell("welcome_media_type", mediaType);
                        Log.Information("Save media success..");

                        await _telegramProvider.EditAsync("Welcome Media berhasil di simpan.");
                        return;
                    }
                    else
                    {
                        await _telegramProvider.SendTextAsync("Media tidak terdeteksi di pesan yg di reply tersebut.");
                        return;
                    }
                }

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

                            // Log.Information(columnTarget);
                            // Log.Information(data);

                            await _telegramProvider.SendTextAsync("Sedang menyimpan Welcome Message..");

                            await _settingsService.UpdateCell(columnTarget, data);
                            await _telegramProvider.EditAsync($"Welcome {target} berhasil di simpan!");
                        }
                        else
                        {
                            await _telegramProvider.SendTextAsync("Masukan Pesan atau tombol yang akan di tetapkan");
                        }
                    }
                    else
                    {
                        await _telegramProvider.SendTextAsync(missParamText);
                    }
                }
                else
                {
                    await _telegramProvider.SendTextAsync(missParamText);
                }
            }
        }
    }
}