using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace WinTenBot.Handlers.Commands.Core
{
    public class InfoCommand : CommandBase
    {
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;

            var sendText = "<b>WinTenBot (.NET) Alpha Preview</b>\n" +
                "Version: 3.0.1037 EAP\n\n" +
                "ℹ️ Official Telegram bot based on <b>WinTen API.</b> for management & utility group.\n\n" +

                "<b>I'm still beta, maybe contains bug and unstable. not recomended for your group.</b>\n\n" +

                "For more fast and keep fast Bot and still continue reability and improvement, please <b>Donate</b> below for buy VPS and give me coffe.\n\n" +

                "I'm still exist run and fast thanks to <b>Akmal Projext</b>";

            var inlineKeyboard = new InlineKeyboardMarkup(new[]{
                new [] {
                    InlineKeyboardButton.WithUrl("👥 WinTen Group", "https://t.me/WinTenGroup"),
                    InlineKeyboardButton.WithUrl("❤️ WinTen Dev", "https://t.me/WinTenDev")},
                new [] {
                    InlineKeyboardButton.WithUrl("👥 Redmi 5A (ID)", "https://t.me/Redmi5AID"),
                    InlineKeyboardButton.WithUrl("👥 Telegram Bot API", "https://t.me/TgBotID")
                },
                new [] {
                    InlineKeyboardButton.WithUrl("💽 Source Code (.NET)", "https://github.com/WinTenDev/WinTenBot.NET"),
                    InlineKeyboardButton.WithUrl("🏗 Akmal Projext", "https://t.me/AkmalProjext")
                },
                new [] {
                    InlineKeyboardButton.WithUrl("💰 Donate", "http://paypal.me/Azhe403")
                }
            });

            await context.Bot.Client.SendTextMessageAsync(
                msg.Chat,
                sendText,
                ParseMode.Html,
                replyMarkup: inlineKeyboard
            );
        }
    }
}