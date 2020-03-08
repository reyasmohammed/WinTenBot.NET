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
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            _rssService = new RssService(_telegramProvider.Message);
            var msg = _telegramProvider.Message;
            var msgId = msg.MessageId;
            var chatId = msg.Chat.Id;
            var msgText = msg.Text;
            var dateDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var isAdminOrPrivate = await _telegramProvider.IsAdminOrPrivateChat();
            if (!isAdminOrPrivate)
            {
                var send = "Maaf, hanya Admin yang dapat mengimport daftar RSS";
                await _telegramProvider.SendTextAsync(send);
            }

            await _telegramProvider.AppendTextAsync("Sedang mempersiapkan");
            var filePath = $"{chatId}/rss-feed_{dateDate}_{msgId}.txt";
            filePath = await _telegramProvider.DownloadFileAsync(filePath);

            await _telegramProvider.AppendTextAsync("Sedang membuka berkas");
            var rssLists = File.ReadAllLines(filePath);
            foreach (var rssList in rssLists)
            {
                Log.Information($"Importing {rssList}");
                var data = new Dictionary<string, object>()
                {
                    {"url_feed", rssList},
                    {"chat_id", _telegramProvider.Message.Chat.Id},
                    {"from_id", _telegramProvider.Message.From.Id}
                };

                await _rssService.SaveRssSettingAsync(data);
            }

            await _telegramProvider.AppendTextAsync($"Memeriksa RSS duplikat");
            _rssService.DeleteDuplicateAsync();

            await _telegramProvider.AppendTextAsync($"{rssLists.Length} RSS berhasil di import");
        }
    }
}