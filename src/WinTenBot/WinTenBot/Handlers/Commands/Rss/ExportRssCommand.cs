using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Enums;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class ExportRssCommand : CommandBase
    {
        private RssService _rssService;
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            _rssService = new RssService(_telegramProvider.Message);
            var msg = _telegramProvider.Message;
            var chatId = msg.Chat.Id;
            var msgId = msg.MessageId;
            var msgText = msg.Text;
            var dateDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var isAdminOrPrivate = await _telegramProvider.IsAdminOrPrivateChat();
            if (!isAdminOrPrivate)
            {
                var send = "Maaf, hanya Admin yang dapat mengekspor daftar RSS";
                await _telegramProvider.SendTextAsync(send);
                return;
            }

            var rssSettings = await _rssService.GetRssSettingsAsync();
            Log.Information($"RssSettings: {rssSettings.ToJson(true)}");

            var listRss = new StringBuilder();
            foreach (var rss in rssSettings)
            {
                listRss.AppendLine(rss.UrlFeed);
            }

            Log.Information($"ListUrl: \n{listRss}");

            var listRssStr = listRss.ToString().Trim();
            var sendText = "Daftar RSS ini tidak terenkripsi, dapat di pulihkan di obrolan mana saja. " +
                           "Tambahkan parameter -e agar daftar RSS terenkripsi.";

            if (msgText.Contains("-e"))
            {
                Log.Information("List RSS will be encrypted.");
                listRssStr = listRssStr.AesEncrypt();
                sendText = "Daftar RSS ini terenkripsi, hanya dapat di pulihkan di obrolan ini!";
            }

            var filePath = $"{chatId}/rss-feed_{dateDate}_{msgId}.txt";
            await listRssStr.WriteTextAsync(filePath);

            var fileSend = IoHelper.BaseDirectory + $"/{filePath}";
            await _telegramProvider.SendMediaAsync(fileSend, MediaType.LocalDocument, sendText);

            fileSend.DeleteFile();
        }
    }
}