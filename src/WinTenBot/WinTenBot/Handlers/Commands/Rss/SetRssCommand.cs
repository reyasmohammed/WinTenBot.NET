using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class SetRssCommand:CommandBase
    {
        private RssService _rssService;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        { 
            ChatHelper.Init(context);
            _rssService = new RssService(ChatHelper.Message);

            var url = ChatHelper.Message.Text.GetTextWithoutCmd();
            if (url != null)
            {
                await $"Sedang memeriksa {url}".AppendTextAsync();
                if (url.CheckUrlValid())
                {
                    await $"Sedang memvalidasi {url}".AppendTextAsync();
                    var isValid = await url.IsValidUrlFeed();
                    if (!isValid)
                    {
                        await "Sedang mencari kemungkinan RSS yang valid".AppendTextAsync();
                        var foundUrl = await url.GetBaseUrl().FindUrlFeed();
                        ConsoleHelper.WriteLine($"Found URL Feed: {foundUrl}");
                        
                        if (foundUrl != "")
                        {
                            url = foundUrl;
                        }
                        else
                        {
                            var notfoundRss = $"Kami tidak dapat memvalidasi {url} adalah Link RSS yang valid, " +
                                              $"dan mencoba mencari di {url.GetBaseUrl()} tetap tidak dapat menemukan.";
                            
                            await notfoundRss.EditAsync();
                            
                            ChatHelper.Close();
                            return;
                        }

                    }
                    
                    var isFeedExist = await _rssService.IsExistRssAsync(url);

                    ConsoleHelper.WriteLine($"Is Url Exist: {isFeedExist}");
                    
                    if (!isFeedExist)
                    {
                        await $"Sedang menyimpan..".SendTextAsync();

                        var data = new Dictionary<string, object>()
                        {
                            {"url_feed", url},
                            {"chat_id", ChatHelper.Message.Chat.Id},
                            {"from_id", ChatHelper.Message.From.Id}
                        };

                        await _rssService.SaveRssSettingAsync(data);
                        await $"Url: {url} berhasil di simpan".EditAsync();

                    }
                    else
                    {
                        await $"Url: {url} sudah di simpan".SendTextAsync();
                    }
                }
                else
                {
                    await "Url tersebut sepertinya tidak valid".SendTextAsync();
                }
            }else
            {
                await "Apa url Feednya?".SendTextAsync();
            }

            ChatHelper.Close();
    }
}
        }