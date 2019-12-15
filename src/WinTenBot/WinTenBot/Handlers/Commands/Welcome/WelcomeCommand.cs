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
    public class WelcomeCommand : CommandBase
    {
        private readonly SettingsService _settingsService;
        private ChatProcessor _chatProcessor;

        public WelcomeCommand()
        {
            _settingsService = new SettingsService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            var msg = context.Update.Message;

            ConsoleHelper.WriteLine($"Args: {string.Join(" ", args)}");
            var sendText = "Perintah /welcome hanya untuk grup saja";

            if (msg.Chat.Type != ChatType.Private)
            {
                var chatTitle = msg.Chat.Title;
                var settings = await _settingsService.GetSettingByGroup(msg.Chat.Id);
                var welcomeMessage = settings.Rows[0]["welcome_message"].ToString();
                var welcomeButton = settings.Rows[0]["welcome_button"].ToString();
                var welcomeMedia = settings.Rows[0]["welcome_media"].ToString();
                var welcomeMediaType = settings.Rows[0]["welcome_media_type"].ToString();
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
                    await _chatProcessor.SendMediaAsync(welcomeMedia, welcomeMediaType, welcomeMessage, keyboard);
                }
                else
                {
                    await _chatProcessor.SendAsync(sendText, keyboard);
                }
            }
            else
            {
                await _chatProcessor.SendAsync(sendText);
            }
        }
    }
}