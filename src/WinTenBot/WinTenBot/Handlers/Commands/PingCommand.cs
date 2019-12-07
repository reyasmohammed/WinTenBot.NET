using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers.Commands
{
    internal class PingCommand : CommandBase
    {
        private ChatProcessor _chatProcessor;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            Message msg = context.Update.Message;

            var keyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData("Ping", "PONG")
            );
            await _chatProcessor.SendAsync("Pong!!", keyboard);

            //            await context.Bot.Client.SendTextMessageAsync(
            //                msg.Chat,
            //                "*PONG*",
            //                ParseMode.Markdown,
            //                replyToMessageId: msg.MessageId,
            //                replyMarkup: new InlineKeyboardMarkup(
            //                    InlineKeyboardButton.WithCallbackData("Ping", "PONG")
            //                )
            //            );
        }
    }
}