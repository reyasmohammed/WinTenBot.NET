using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Additional
{
    public class TesseractOcrCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);

            var msg = _telegramProvider.Message;
            var chatId = msg.Chat.Id;
            var msgId = msg.MessageId;

            if (msg.ReplyToMessage != null)
            {
                var repMsg = msg.ReplyToMessage;
                if (repMsg.Photo != null)
                {
                    await _telegramProvider.SendTextAsync("Sedang memproses gambar");
                    var fileName = $"{chatId}/ocr-{msgId}.jpg";
                    Log.Information("Preparing photo");
                    var savedFile = await _telegramProvider.DownloadFileAsync(fileName);

                    // var ocr = TesseractProvider.ScanImage(savedFile);
                    var ocr = await TesseractProvider.OcrSpace(savedFile);

                    var txt = @$"<b>Scan Result</b>\n{ocr}";
                    await _telegramProvider.EditAsync(ocr);
                    return;
                }
            }

            await _telegramProvider.SendTextAsync("Silakan reply salah satu gambar");
        }
    }
}