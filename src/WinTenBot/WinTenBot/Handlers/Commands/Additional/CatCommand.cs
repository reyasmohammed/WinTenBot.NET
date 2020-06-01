using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WinTenBot.Model;
using WinTenBot.Services;
using WinTenBot.Text;
using sysIO = System.IO;

namespace WinTenBot.Handlers.Commands.Additional
{
    public class CatCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            var client = _telegramService.Client;
            var message = _telegramService.Message;
            var partsText = message.Text.SplitText(" ").ToArray();
            var chatId = message.Chat.Id;
            var listAlbum = new List<IAlbumInputMedia>();
            var catSource = "http://aws.random.cat/meow";
            var catNum = 1;
            var param1 = partsText.ValueOfIndex(1);

            if (param1.IsNotNullOrEmpty())
            {
                if (!param1.IsNumeric())
                {
                    await _telegramService.SendTextAsync("Pastikan jumlah kochenk yang diminta berupa angka.")
                        .ConfigureAwait(false);
                    return;
                }

                catNum = param1.ToInt();

                if (catNum > 10)
                {
                    await _telegramService.SendTextAsync("Batas maksimal Kochenk yg di minta adalah 10")
                        .ConfigureAwait(false);
                    return;
                }
            }

            await _telegramService.SendTextAsync($"Sedang mempersiapkan {catNum} Kochenk")
                .ConfigureAwait(false);
            for (int i = 1; i <= catNum; i++)
            {
                Log.Information($"Loading cat {i} of {catNum} from {catSource}");
                // await _telegramService.EditAsync($"Sedang mengambil {i} of {catNum} Kochenk")
                //     .ConfigureAwait(false);

                var url = await catSource
                    .GetJsonAsync<CatMeow>(cancellationToken)
                    .ConfigureAwait(false);
                var urlFile = url.File.AbsoluteUri;

                Log.Information($"Adding kochenk {urlFile}");

                var fileName = sysIO.Path.GetFileName(urlFile);
                var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd");
                var saveName = sysIO.Path.Combine(chatId.ToString(), $"kochenk_{timeStamp}_" + fileName);
                var savedPath = urlFile.SaveToCache(saveName);

                var fileStream = sysIO.File.OpenRead(savedPath);
                listAlbum.Add(new InputMediaPhoto()
                {
                    Caption = $"Kochenk {i}",
                    Media = new InputMedia(urlFile),
                    ParseMode = ParseMode.Html
                });

                // listAlbum.Add(new InputMediaPhoto(new InputMedia()));
                await fileStream.DisposeAsync().ConfigureAwait(false);

                Thread.Sleep(100);
            }

            await _telegramService.DeleteAsync().ConfigureAwait(false);
            await client.SendMediaGroupAsync(listAlbum, chatId, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}