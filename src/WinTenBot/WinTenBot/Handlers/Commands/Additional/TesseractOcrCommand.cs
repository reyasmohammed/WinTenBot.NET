using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Additional
{
    public class TesseractOcrCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);

            var msg = _telegramService.Message;
            var chatId = msg.Chat.Id;
            var msgId = msg.MessageId;

            if (msg.ReplyToMessage != null)
            {
                var repMsg = msg.ReplyToMessage;
                if (repMsg.Photo != null)
                {
                    await _telegramService.SendTextAsync("Sedang memproses gambar");
                    var fileName = $"{chatId}/ocr-{msgId}.jpg";
                    Log.Information("Preparing photo");
                    var savedFile = await _telegramService.DownloadFileAsync(fileName);

                    // var ocr = TesseractProvider.ScanImage(savedFile);
                    var ocr = await TesseractProvider.OcrSpace(savedFile);

                    var txt = @$"<b>Scan Result</b>\n{ocr}";
                    await _telegramService.EditAsync(ocr);
                    return;
                }
            }

            await _telegramService.SendTextAsync("Silakan reply salah satu gambar");
        }
    }
}