using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CodeHollow.FeedReader;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using WinTenBot.Model;
using WinTenBot.Services;

namespace WinTenBot.Helpers
{
    public static class RssHelper
    {
        public static async Task<int> ExecBroadcasterAsync(string chatId)
        {
            ConsoleHelper.WriteLine("Starting RSS Scheduler.");
            int newRssCount = 0;

            var rssService = new RssService();

            ConsoleHelper.WriteLine("Getting RSS settings..");
            var rssSettings = await rssService.GetRssSettingsAsync(chatId);
            foreach (RssSetting rssSetting in rssSettings)
            {
                var rssUrl = rssSetting.UrlFeed;
                
                // var rssUrl = rssSetting["url_feed"].ToString();
                // var chatId = rssSetting["chat_id"].ToString();

                ConsoleHelper.WriteLine($"Processing {rssUrl} for {chatId}.");
                try
                {
                    var rssFeeds = await FeedReader.ReadAsync(rssUrl);
                    var rssTitle = rssFeeds.Title;

                    var castLimit = 3;
                    var castStep = 0;

                    foreach (var rssFeed in rssFeeds.Items)
                    {
                        // Prevent flood in first time;
                        if (castLimit == castStep)
                        {
                            ConsoleHelper.WriteLine($"Send stopped due limit {castLimit} for prevent flooding in first time");
                            break;
                        }

                        var titleLink = $"{rssTitle} - {rssFeed.Title}".MkUrl(rssFeed.Link);
                        var category = rssFeed.Categories.MkJoin(", ");
                        var sendText = $"{titleLink}" +
                                       $"\nTags: {category}";

                        var where = new Dictionary<string, object>()
                        {
                            {"chat_id", chatId},
                            {"url", rssFeed.Link}
                        };

                        var isExist = await rssService.IsExistInHistory(where);
                        if (!isExist)
                        {
                            ConsoleHelper.WriteLine($"Sending feed to {chatId}");

                            try
                            {
                                await Bot.Client.SendTextMessageAsync(chatId, sendText, ParseMode.Html);

                                var data = new Dictionary<string, object>()
                                {
                                    {"url", rssFeed.Link},
                                    {"rss_source", rssUrl},
                                    {"chat_id", chatId},
                                    {"title", rssFeed.Title},
                                    {"publish_date", rssFeed.PublishingDate.ToString()},
                                    {"author", rssFeed.Author}
                                };

                                ConsoleHelper.WriteLine($"Writing to RSS History");
                                await rssService.SaveRssAsync(data);

                                castStep++;
                                newRssCount++;
                            }
                            catch (ChatNotFoundException chatNotFoundException)
                            {
                                ConsoleHelper.WriteLine($"May Bot not added in {chatId}." +
                                                        $"\n{chatNotFoundException.Message}");
                            }
                        }
                        else
                        {
                            ConsoleHelper.WriteLine($"This feed has sent to {chatId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLine(ex.Message);
                    ConsoleHelper.WriteLine(ex.ToString());
                }
            }

            ConsoleHelper.WriteLine($"RSS Scheduler finished. New RSS Count: {newRssCount}");

            return newRssCount;
        }
    }
}