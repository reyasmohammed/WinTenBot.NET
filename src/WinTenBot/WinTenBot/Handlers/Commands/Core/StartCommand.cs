using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Model;

namespace WinTenBot.Handlers.Commands
{
    class StartCommand : CommandBase
    {
        private ChatProcessor _chatProcessor;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);

            var botName = Bot.GlobalConfiguration["Engines:ProductName"];
            var botVer = Bot.GlobalConfiguration["Engines:Version"];
            var botCompany = Bot.GlobalConfiguration["Engines:Company"];
            
            string sendText = $"🤖 {botName} {botVer}" +
                           $"\nby {botCompany}." +
                           $"\nAdalah bot debugging, manajemen grup yang di lengkapi dengan alat keamanan. " +
                           $"Agar fungsi saya bekerja dengan fitur penuh, jadikan saya admin dengan level standard. " +
                           $"\nSaran dan fitur bisa di ajukan di @WinTenGroup atau @TgBotID.";

            var keyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData("Bantuan", "help")
            );
            
            await _chatProcessor.SendAsync(sendText,keyboard);

//            await context.Bot.Client.SendTextMessageAsync(context.Update.Message.Chat, "Hello, World!");
//            await next(context);
        }
    }
}