using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Welcome
{
    public class WelcomeCommand:CommandBase
    {
        private readonly SettingsService _settingsService;
        public WelcomeCommand()
        {
            _settingsService = new SettingsService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;

            Console.WriteLine($"Args: {string.Join(" ",args)}");
            var sendText = "Perintah /welcome hanya untuk grup saja";
            IReplyMarkup keyboard = new ReplyKeyboardMarkup();

            if (msg.Chat.Type != ChatType.Private)
            {
                var chatTitle = msg.Chat.Title;
                var settings = await _settingsService.GetSettingByGrup(msg.Chat.Id);
                var welcomeMessage = settings.Rows[0]["welcome_message"].ToString();
                var welcomeButton = settings.Rows[0]["welcome_button"].ToString();
                var splitWelcomeButton = welcomeButton.Split(',').ToList<string>();

                keyboard = KeyboardHelper.ToReplyMarkup(welcomeButton, 2);
                sendText = $"👥 <b>{chatTitle}</b>\n";
                if (welcomeMessage == "")
                {
                    sendText += "Tidak ada konfigurasi pesan welcome, pesan default akan di terapkan";
                }

                if (args[0] == "anu")
                {
                    sendText += " anu";
                }

//                sendText += " " + string.Join(", ",args);
            }

            await context.Bot.Client.SendTextMessageAsync(
                msg.Chat,
                sendText,
                ParseMode.Html,
                replyMarkup: keyboard,
                replyToMessageId: msg.MessageId,
                cancellationToken: cancellationToken);
        }
    }
}