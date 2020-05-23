using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Core
{
    public class HelpCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);

            var sendText = "Untuk mendapatkan bantuan klik tombol dibawah ini";
            var urlStart = await _telegramService.GetUrlStart("start=help");
            var keyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl("Dapatkan bantuan", urlStart)
            );

            if (_telegramService.IsPrivateChat())
            {
                sendText = await "home".LoadInBotDocs();
                keyboard = await "Storage/Buttons/home.json".JsonToButton();
            }

            await _telegramService.SendTextAsync(sendText, keyboard);
        }
    }
}