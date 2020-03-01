using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Scheduler;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class SetRssCommand : CommandBase
    {
        private RssService _rssService;
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            _rssService = new RssService(_telegramProvider.Message);
            var chatId = _telegramProvider.Message.Chat.Id;

            var url = _telegramProvider.Message.Text.GetTextWithoutCmd();
            if (url != null)
            {
                await _telegramProvider.AppendTextAsync($"URL: {url}");

                if (url.CheckUrlValid())
                {
                    await _telegramProvider.AppendTextAsync($"Sedang mengecek apakah berisi RSS");
                    var isValid = await url.IsValidUrlFeed();
                    if (!isValid)
                    {
                        await _telegramProvider.AppendTextAsync("Sedang mencari kemungkinan tautan RSS yang valid");
                        var foundUrl = await url.GetBaseUrl().FindUrlFeed();
                        Log.Information($"Found URL Feed: {foundUrl}");

                        if (foundUrl != "")
                        {
                            url = foundUrl;
                        }
                        else
                        {
                            var notfoundRss = $"Kami tidak dapat memvalidasi {url} adalah Link RSS yang valid, " +
                                              $"dan mencoba mencari di {url.GetBaseUrl()} tetap tidak dapat menemukan.";

                            await _telegramProvider.EditAsync(notfoundRss);
                            return;
                        }
                    }

                    var isFeedExist = await _rssService.IsExistRssAsync(url);

                    Log.Information($"Is Url Exist: {isFeedExist}");

                    if (!isFeedExist)
                    {
                        await _telegramProvider.AppendTextAsync($"Sedang menyimpan..");

                        var data = new Dictionary<string, object>()
                        {
                            {"url_feed", url},
                            {"chat_id", _telegramProvider.Message.Chat.Id},
                            {"from_id", _telegramProvider.Message.From.Id}
                        };

                        await _rssService.SaveRssSettingAsync(data);

                        await _telegramProvider.AppendTextAsync("Memastikan Scheduler sudah berjalan");
                        chatId.RegisterScheduler();

                        await _telegramProvider.AppendTextAsync($"Tautan berhasil di simpan");
                    }
                    else
                    {
                        await _telegramProvider.AppendTextAsync($"Tautan sudah di simpan");
                    }
                }
                else
                {
                    await _telegramProvider.AppendTextAsync("Url tersebut sepertinya tidak valid");
                }
            }
            else
            {
                await _telegramProvider.SendTextAsync("Apa url Feednya?");
            }
        }
    }
}