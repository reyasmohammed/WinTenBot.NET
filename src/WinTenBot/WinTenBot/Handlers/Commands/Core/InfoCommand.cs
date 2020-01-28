using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    public class InfoCommand : CommandBase
    {
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);

            // Message msg = context.Update.Message;

            var sendText = "<b>WinTenBot (.NET) Alpha Preview</b>\n" +
                           "Version: 3.0.1037 EAP\n\n" +
                           "ℹ️ Bot Telegram resmi berbasis <b>WinTen API.</b> untuk manajemen dan peralatan grup.\n\n" +
                           "<b>Saya masih Beta, mungkin terdapat bug dan tidak stabil. Tidak di rekomendasikan untuk grup Anda.</b>\n\n" +
                           "Untuk Bot lebih cepat dan tetap cepat dan terus peningkatan dan keandalan, silakan <b>Donasi</b> via Paypal untuk beli VPS dan beri saya Kopi.\n\n" +
                           "Saya tetap ada dan masih berjalan cepat, terima kasih banyak untuk <b>Akmal Projext</b>.";

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithUrl("👥 WinTen Group", "https://t.me/WinTenGroup"),
                    InlineKeyboardButton.WithUrl("❤️ WinTen Dev", "https://t.me/WinTenDev")
                },
                new[]
                {
                    InlineKeyboardButton.WithUrl("👥 Redmi 5A (ID)", "https://t.me/Redmi5AID"),
                    InlineKeyboardButton.WithUrl("👥 Telegram Bot API", "https://t.me/TgBotID")
                },
                new[]
                {
                    InlineKeyboardButton.WithUrl("💽 Source Code (.NET)", "https://github.com/WinTenDev/WinTenBot.NET"),
                    InlineKeyboardButton.WithUrl("🏗 Akmal Projext", "https://t.me/AkmalProjext")
                },
                new[]
                {
                    InlineKeyboardButton.WithUrl("💰 Donate", "http://paypal.me/Azhe403")
                }
            });

            await _requestProvider.SendTextAsync(sendText, inlineKeyboard);

            // await context.Bot.Client.SendTextMessageAsync(
            //     msg.Chat,
            //     sendText,
            //     ParseMode.Html,
            //     replyMarkup: inlineKeyboard
            // );
        }
    }
}