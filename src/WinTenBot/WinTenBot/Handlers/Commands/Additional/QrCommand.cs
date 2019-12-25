using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers.Commands.Additional
{
    public class QrCommand:CommandBase
    {
        private ChatProcessor _chatProcessor;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            var msg = context.Update.Message;
            var data = msg.Text.GetTextWithoutCmd();
            
            if (msg.ReplyToMessage != null)
            {
                data = msg.ReplyToMessage.Text?? msg.ReplyToMessage.Caption;

            }

            if (data == "")
            {
                var sendText = "<b>Generate QR from text or caption media</b>" +
                        "\n<b>Usage : </b><code>/qr</code> (In-Reply)" +
                        "\n                <code>/qr your text here</code> (In-Message)";
                await _chatProcessor.SendAsync(sendText);
                return;
            }

            var urlQr = data.GenerateUrlQrApi();
            await _chatProcessor.SendMediaAsync(urlQr,"photo");
        }
    }
}