using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    public class HelpCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);

            var sendText = "Untuk mendapatkan bantuan klik tombol dibawah ini";
            var urlStart = await _telegramProvider.GetUrlStart("start=help");
            var keyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl("Dapatkan bantuan", urlStart)
            );

            if (_telegramProvider.IsPrivateChat())
            {
                sendText = await "home".LoadInBotDocs();
                keyboard = await "Storage/Buttons/home.json".JsonToButton();
            }

            await _telegramProvider.SendTextAsync(sendText, keyboard);
        }
    }
}