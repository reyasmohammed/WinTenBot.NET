using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Welcome
{
    public class SetWelcomeCommand : CommandBase
    {
        private RequestProvider _requestProvider;
        private SettingsService _settingsService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;
            _settingsService = new SettingsService(msg);

            if (msg.Chat.Type == ChatType.Private)
            {
                await _requestProvider.SendTextAsync("Welcome hanya untuk grup saja");
                return;
            }

            var partsMsg = msg.Text.Split(' ').ToArray();

            string[] commands = {"message", "msg", "button", "btn"};
            ConsoleHelper.WriteLine(partsMsg.ToJson());

            var isAdmin = await _requestProvider.IsAdminGroup();
            if (isAdmin)
            {
                if (msg.ReplyToMessage != null)
                {
                    var repMsg = msg.ReplyToMessage;
                    if (repMsg.GetFileId() != "")
                    {
                        var mediaType = repMsg.Type.ToString().ToLower();
                        await _requestProvider.SendTextAsync("Sedang menyimpan Welcome Media..");
                        await _settingsService.UpdateCell("welcome_media", repMsg.GetFileId());
                        await _settingsService.UpdateCell("welcome_media_type", mediaType);
                        ConsoleHelper.WriteLine("Save media success..");

                        await _requestProvider.EditAsync("Welcome Media berhasil di simpan.");
                        return;
                    }
                    else
                    {
                        await _requestProvider.SendTextAsync("Media tidak terdeteksi di pesan yg di reply tersebut.");
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

                            await _requestProvider.SendTextAsync("Sedang menyimpan Welcome Message..");

                            await _settingsService.UpdateCell(columnTarget, data);
                            await _requestProvider.EditAsync($"Welcome {target} berhasil di simpan!");
                        }
                        else
                        {
                            await _requestProvider.SendTextAsync("Masukan Pesan atau tombol yang akan di tetapkan");
                        }
                    }
                    else
                    {
                       
                        await _requestProvider.SendTextAsync(missParamText);
                    }
                }
                else
                {
                    await _requestProvider.SendTextAsync(missParamText);
                }
            }
        }
    }
}