﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Common;
using WinTenBot.Scheduler;
using WinTenBot.Services;
using WinTenBot.Telegram;
using WinTenBot.Tools;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class SetRssCommand : CommandBase
    {
        private RssService _rssService;
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            _rssService = new RssService(_telegramService.Message);
            var chatId = _telegramService.Message.Chat.Id;

            var url = _telegramService.Message.Text.GetTextWithoutCmd();
            if (url != null)
            {
                await _telegramService.AppendTextAsync($"URL: {url}");

                if (url.CheckUrlValid())
                {
                    await _telegramService.AppendTextAsync($"Sedang mengecek apakah berisi RSS");
                    var isValid = await url.IsValidUrlFeed();
                    if (!isValid)
                    {
                        await _telegramService.AppendTextAsync("Sedang mencari kemungkinan tautan RSS yang valid");
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

                            await _telegramService.EditAsync(notfoundRss);
                            return;
                        }
                    }

                    var isFeedExist = await _rssService.IsExistRssAsync(url);

                    Log.Information($"Is Url Exist: {isFeedExist}");

                    if (!isFeedExist)
                    {
                        await _telegramService.AppendTextAsync($"Sedang menyimpan..");

                        var data = new Dictionary<string, object>()
                        {
                            {"url_feed", url},
                            {"chat_id", _telegramService.Message.Chat.Id},
                            {"from_id", _telegramService.Message.From.Id}
                        };

                        await _rssService.SaveRssSettingAsync(data);

                        await _telegramService.AppendTextAsync("Memastikan Scheduler sudah berjalan");
                        chatId.RegisterScheduler();

                        await _telegramService.AppendTextAsync($"Tautan berhasil di simpan");
                    }
                    else
                    {
                        await _telegramService.AppendTextAsync($"Tautan sudah di simpan");
                    }
                }
                else
                {
                    await _telegramService.AppendTextAsync("Url tersebut sepertinya tidak valid");
                }
            }
            else
            {
                await _telegramService.SendTextAsync("Apa url Feednya?");
            }
        }
    }
}