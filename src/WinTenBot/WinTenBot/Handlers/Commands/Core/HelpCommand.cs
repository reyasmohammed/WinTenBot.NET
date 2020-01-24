using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers.Commands.Core
{
    public class HelpCommand:CommandBase
    {
        private ChatProcessor _chatProcessor;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);

            var sendText = "Untuk mendapatkan bantuan klik tombol dibawah ini";
            var urlStart = await "help".GetUrlStart();
            var keyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl("Dapatkan bantuan", urlStart)
            );
            
            if (_chatProcessor.IsPrivateChat())
            {
                sendText = await "home".LoadInBotDocs();
                keyboard = await "Storage/Buttons/home.json".JsonToButton();
            }
            
            await _chatProcessor.SendAsync(sendText, keyboard);

        }
    }
}