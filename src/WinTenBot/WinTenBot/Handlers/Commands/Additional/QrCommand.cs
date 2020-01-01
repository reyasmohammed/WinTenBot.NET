using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers.Commands.Additional
{
    public class QrCommand : CommandBase
    {
        private ChatProcessor _chatProcessor;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            var msg = context.Update.Message;
            Message repMsg = null;
            var data = msg.Text.GetTextWithoutCmd();

            if (msg.ReplyToMessage != null)
            {
                repMsg = msg.ReplyToMessage;
                data = repMsg.Text ?? repMsg.Caption;
            }

            if (data == "")
            {
                var sendText = "<b>Generate QR from text or caption media</b>" +
                               "\n<b>Usage : </b><code>/qr</code> (In-Reply)" +
                               "\n                <code>/qr your text here</code> (In-Message)";
                await _chatProcessor.SendAsync(sendText);
                return;
            }

            InlineKeyboardMarkup keyboard = null;
            if (repMsg != null)
            {
                keyboard = new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithUrl("Sumber", repMsg.GetMessageLink())
                );
            }

            var urlQr = data.GenerateUrlQrApi();
            var fileName = $"{msg.Chat.Id}_{msg.MessageId}.jpg";
            
            // urlQr.SaveUrlTo(fileName);
            await _chatProcessor.SendMediaAsync(urlQr, "photo", replyMarkup: keyboard);
        }
    }
}