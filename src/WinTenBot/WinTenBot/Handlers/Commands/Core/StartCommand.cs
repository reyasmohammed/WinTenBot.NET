using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    class StartCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);

            var botName = Bot.GlobalConfiguration["Engines:ProductName"];
            var botVer = Bot.GlobalConfiguration["Engines:Version"];
            var botCompany = Bot.GlobalConfiguration["Engines:Company"];

            string sendText = $"🤖 {botName} {botVer}" +
                              $"\nby {botCompany}." +
                              $"\nAdalah bot debugging, manajemen grup yang di lengkapi dengan alat keamanan. " +
                              $"Agar fungsi saya bekerja dengan fitur penuh, jadikan saya admin dengan level standard. " +
                              $"\nSaran dan fitur bisa di ajukan di @WinTenGroup atau @TgBotID.";

            // var urlStart = await "help".GetUrlStart();
            var urlStart = await _telegramProvider.GetUrlStart("start=help");
            var urlAddTo = await _telegramProvider.GetUrlStart("startgroup=new");
            var keyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl("Dapatkan bantuan", urlStart)
            );

            if (_telegramProvider.IsPrivateChat())
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

            await _telegramProvider.SendTextAsync(sendText, keyboard);
        }
    }
}