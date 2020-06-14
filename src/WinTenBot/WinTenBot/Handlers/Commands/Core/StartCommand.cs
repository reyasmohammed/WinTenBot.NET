﻿using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Common;
using WinTenBot.Model;
using WinTenBot.Services;
using WinTenBot.Telegram;

namespace WinTenBot.Handlers.Commands.Core
{
    class StartCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            var msg = _telegramService.Message;
            var partText = msg.Text.SplitText(" ").ToArray();
            var paramStart = partText.ValueOfIndex(1);

            var botName = BotSettings.GlobalConfiguration["Engines:ProductName"];
            var botVer = BotSettings.GlobalConfiguration["Engines:Version"];
            var botCompany = BotSettings.GlobalConfiguration["Engines:Company"];

            string sendText = $"🤖 {botName} {botVer}" +
                              $"\nby {botCompany}." +
                              $"\nAdalah bot debugging, manajemen grup yang di lengkapi dengan alat keamanan. " +
                              $"Agar fungsi saya bekerja dengan fitur penuh, jadikan saya admin dengan level standard. " +
                              $"\nSaran dan fitur bisa di ajukan di @WinTenGroup atau @TgBotID.";

            var urlStart = await _telegramService.GetUrlStart("start=help")
                .ConfigureAwait(false);
            var urlAddTo = await _telegramService.GetUrlStart("startgroup=new")
                .ConfigureAwait(false);

            switch (paramStart)
            {
                case "set-username":
                    var setUsername = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("Pasang Username", "https://t.me/WinTenDev/29")
                        }
                    });
                    var send = "Untuk cara pasang Username, silakan klik tombol di bawah ini";
                    await _telegramService.SendTextAsync(send, setUsername)
                        .ConfigureAwait(false);
                    break;

                default:
                    var keyboard = new InlineKeyboardMarkup(
                        InlineKeyboardButton.WithUrl("Dapatkan bantuan", urlStart)
                    );

                    if (_telegramService.IsPrivateChat())
                    {
                        keyboard = new InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Bantuan", "help home"),
                                InlineKeyboardButton.WithUrl("Pasang Username", "https://t.me/WinTenDev/29")
                            },
                            new[]
                            {
                                InlineKeyboardButton.WithUrl("Tambahkan ke Grup", urlAddTo)
                            }
                        });
                    }

                    await _telegramService.SendTextAsync(sendText, keyboard)
                        .ConfigureAwait(false);
                    break;
            }
        }
    }
}