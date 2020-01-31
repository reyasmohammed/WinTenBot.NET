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
    public class WelcomeCommand : CommandBase
    {
        private SettingsService _settingsService;
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;
            _settingsService = new SettingsService(msg);


            ConsoleHelper.WriteLine($"Args: {string.Join(" ", args)}");
            var sendText = "Perintah /welcome hanya untuk grup saja";

            if (msg.Chat.Type != ChatType.Private)
            {
                var chatTitle = msg.Chat.Title;
                var settings = await _settingsService.GetSettingByGroup();
                var welcomeMessage = settings.WelcomeMessage;
                var welcomeButton = settings.WelcomeButton;
                var welcomeMedia = settings.WelcomeMedia;
                var welcomeMediaType = settings.WelcomeMediaType;
                var splitWelcomeButton = welcomeButton.Split(',').ToList<string>();

                var keyboard = welcomeButton.ToReplyMarkup(2);
                sendText = $"👥 <b>{chatTitle}</b>\n";
                if (welcomeMessage == "")
                {
                    sendText += "Tidak ada konfigurasi pesan welcome, pesan default akan di terapkan";
                }
                else
                {
                    sendText += welcomeMessage;
                }

//                if (args[0] == "anu")
//                {
//                    sendText += " anu";
//                }

//                sendText += " " + string.Join(", ",args);
                if (welcomeMediaType != "")
                {
                    await _requestProvider.SendMediaAsync(welcomeMedia, welcomeMediaType, welcomeMessage, keyboard);
                }
                else
                {
                    await _requestProvider.SendTextAsync(sendText, keyboard);
                }
            }
            else
            {
                await _requestProvider.SendTextAsync(sendText);
            }
        }
    }
}