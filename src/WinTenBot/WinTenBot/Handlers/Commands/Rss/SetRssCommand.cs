using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class SetRssCommand : CommandBase
    {
        private RssService _rssService;
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            _rssService = new RssService(_requestProvider.Message);

            var url = _requestProvider.Message.Text.GetTextWithoutCmd();
            if (url != null)
            {
                await _requestProvider.AppendTextAsync($"Sedang memeriksa {url}");
                if (url.CheckUrlValid())
                {
                    await _requestProvider.AppendTextAsync($"Sedang memvalidasi {url}");
                    var isValid = await url.IsValidUrlFeed();
                    if (!isValid)
                    {
                        await _requestProvider.AppendTextAsync("Sedang mencari kemungkinan RSS yang valid");
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

                            await _requestProvider.EditAsync(notfoundRss);
                            return;
                        }
                    }

                    var isFeedExist = await _rssService.IsExistRssAsync(url);

                    Log.Information($"Is Url Exist: {isFeedExist}");

                    if (!isFeedExist)
                    {
                        await _requestProvider.SendTextAsync($"Sedang menyimpan..");

                        var data = new Dictionary<string, object>()
                        {
                            {"url_feed", url},
                            {"chat_id", _requestProvider.Message.Chat.Id},
                            {"from_id", _requestProvider.Message.From.Id}
                        };

                        await _rssService.SaveRssSettingAsync(data);
                        await _requestProvider.EditAsync($"Url: {url} berhasil di simpan");
                    }
                    else
                    {
                        await _requestProvider.SendTextAsync($"Url: {url} sudah di simpan");
                    }
                }
                else
                {
                    await _requestProvider.SendTextAsync("Url tersebut sepertinya tidak valid");
                }
            }
            else
            {
                await _requestProvider.SendTextAsync("Apa url Feednya?");
            }
        }
    }
}