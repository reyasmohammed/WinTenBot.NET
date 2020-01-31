using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeHollow.FeedReader;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using SqlKata.Extensions;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Helpers
{
    public static class RssHelper
    {
        public static async Task<int> ExecBroadcasterAsync(string chatId)
        {
            Log.Information("Starting RSS Scheduler.");
            int newRssCount = 0;

            var rssService = new RssService();

            Log.Information("Getting RSS settings..");
            var rssSettings = await rssService.GetRssSettingsAsync(chatId);

            var tasks = rssSettings.Select(async rssSetting =>
            {
                // foreach (RssSetting rssSetting in rssSettings)
                // {
                var rssUrl = rssSetting.UrlFeed;

                ConsoleHelper.WriteLine($"Processing {rssUrl} for {chatId}.");
                try
                {
                    var rssFeeds = await FeedReader.ReadAsync(rssUrl);
                    var rssTitle = rssFeeds.Title;

                    var castLimit = 1;
                    var castStep = 0;

                    foreach (var rssFeed in rssFeeds.Items)
                    {
                        // Prevent flood in first time;
                        if (castLimit == castStep)
                        {
                            Log.Information($"Send stopped due limit {castLimit} for prevent flooding in first time");
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
                            Log.Information($"Sending feed to {chatId}");

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
                                    {"author", rssFeed.Author},
                                    {"created_at", DateTime.Now.ToString()}
                                };

                                Log.Information($"Writing to RSS History");
                                await rssService.SaveRssHistoryAsync(data);

                                castStep++;
                                newRssCount++;
                            }
                            catch (ChatNotFoundException chatNotFoundException)
                            {
                                Log.Information($"May Bot not added in {chatId}.");
                                Log.Error(chatNotFoundException, "Chat Not Found");
                                // ConsoleHelper.WriteLine($"May Bot not added in {chatId}." +
                                // $"\n{chatNotFoundException.Message}");
                            }
                        }
                        else
                        {
                            Log.Information($"This feed has sent to {chatId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Broadcasting RSS Feed.");
                    Thread.Sleep(4000);

                    // ConsoleHelper.WriteLine(ex.Message);
                    // ConsoleHelper.WriteLine(ex.ToString());
                }

                // }
            });

            await Task.WhenAll(tasks);

            Log.Information($"RSS Scheduler finished. New RSS Count: {newRssCount}");

            return newRssCount;
        }

        public static async Task<string> FindUrlFeed(this string url)
        {
            Log.Information($"Scanning {url} ..");
            var urls = await FeedReader.GetFeedUrlsFromUrlAsync(url);
            Log.Information($"UrlFeeds: {urls.ToJson()}");

            string feedUrl = "";

            if (urls.Count() == 1) // no url - probably the url is already the right feed url
                feedUrl = url;
            else if (urls.Count() == 1)
                feedUrl = urls.First().Url;
            else if (urls.Count() == 2
            ) // if 2 urls, then its usually a feed and a comments feed, so take the first per default
                feedUrl = urls.First().Url;

            return feedUrl;
        }

        public static async Task<bool> IsValidUrlFeed(this string url)
        {
            bool isValid = false;
            try
            {
                var feed = await FeedReader.ReadAsync(url);
                // ConsoleHelper.WriteLine(feed.ToJson());
                isValid = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Validating RSS Feed");
                // ConsoleHelper.WriteLine(ex.Message);
                // ConsoleHelper.WriteLine(ex.ToString());
            }

            Log.Debug($"{url} IsValidUrlFeed: {isValid}");

            return isValid;
        }
    }
}