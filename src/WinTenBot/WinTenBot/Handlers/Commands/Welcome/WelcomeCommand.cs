using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Enums;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Welcome
{
    public class WelcomeCommand : CommandBase
    {
        private SettingsService _settingsService;
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            var msg = context.Update.Message;
            _settingsService = new SettingsService(msg);


            Log.Information($"Args: {string.Join(" ", args)}");
            var sendText = "Perintah /welcome hanya untuk grup saja";

            if (msg.Chat.Type != ChatType.Private)
            {
                var chatTitle = msg.Chat.Title;
                var settings = await _settingsService.GetSettingByGroup();
                var welcomeMessage = settings.WelcomeMessage;
                var welcomeButton = settings.WelcomeButton;
                var welcomeMedia = settings.WelcomeMedia;
                var welcomeMediaType = settings.WelcomeMediaType;
                // var splitWelcomeButton = welcomeButton.Split(',').ToList<string>();

                // var keyboard = welcomeButton.ToReplyMarkup(2);
                InlineKeyboardMarkup keyboard = null;
                if (!welcomeButton.IsNullOrEmpty())
                {
                    keyboard = welcomeButton.ToReplyMarkup(2);
                }
                
                sendText = $"👥 <b>{chatTitle}</b>\n";
                if (welcomeMessage.IsNullOrEmpty())
                {
                    sendText += "Tidak ada konfigurasi pesan welcome, pesan default akan di terapkan";
                }
                else
                {
                    sendText += welcomeMessage;
                }

//                sendText += " " + string.Join(", ",args);
                if (!welcomeMediaType.IsNullOrEmpty())
                {
                    await _telegramProvider.SendMediaAsync(welcomeMedia, MediaType.Document, welcomeMessage, keyboard);
                }
                else
                {
                    await _telegramProvider.SendTextAsync(sendText, keyboard);
                }
            }
            else
            {
                await _telegramProvider.SendTextAsync(sendText);
            }
        }
    }
}