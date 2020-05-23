using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class ImportRssCommand : CommandBase
    {
        private RssService _rssService;
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            _rssService = new RssService(_telegramService.Message);
            var msg = _telegramService.Message;
            var msgId = msg.MessageId;
            var chatId = msg.Chat.Id;
            var msgText = msg.Text;
            var dateDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var isAdminOrPrivate = await _telegramService.IsAdminOrPrivateChat();
            if (!isAdminOrPrivate)
            {
                var send = "Maaf, hanya Admin yang dapat mengimport daftar RSS";
                await _telegramService.SendTextAsync(send);
            }

            await _telegramService.AppendTextAsync("Sedang mempersiapkan");
            var filePath = $"{chatId}/rss-feed_{dateDate}_{msgId}.txt";
            filePath = await _telegramService.DownloadFileAsync(filePath);

            await _telegramService.AppendTextAsync("Sedang membuka berkas");
            var rssLists = File.ReadAllLines(filePath);
            foreach (var rssList in rssLists)
            {
                Log.Information($"Importing {rssList}");
                var data = new Dictionary<string, object>()
                {
                    {"url_feed", rssList},
                    {"chat_id", _telegramService.Message.Chat.Id},
                    {"from_id", _telegramService.Message.From.Id}
                };

                await _rssService.SaveRssSettingAsync(data);
            }

            await _telegramService.AppendTextAsync($"Memeriksa RSS duplikat");
            await _rssService.DeleteDuplicateAsync();

            await _telegramService.AppendTextAsync($"{rssLists.Length} RSS berhasil di import");
        }
    }
}