﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeHollow.FeedReader;
using Serilog;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using WinTenBot.Common;
using WinTenBot.Model;
using WinTenBot.Services;

namespace WinTenBot.Tools
{
    public static class RssBroadcaster
    {
        public static async Task<int> ExecBroadcasterAsync(long chatId)
        {
            Log.Information("Starting RSS Scheduler.");
            int newRssCount = 0;

            var rssService = new RssService();

            Log.Information("Getting RSS settings..");
            var rssSettings = await rssService.GetRssSettingsAsync(chatId)
                .ConfigureAwait(false);

            var tasks = rssSettings.Select(async rssSetting =>
            {
                // foreach (RssSetting rssSetting in rssSettings)
                // {
                var rssUrl = rssSetting.UrlFeed;

                Log.Information($"Processing {rssUrl} for {chatId}.");
                try
                {
                    await ExecuteUrlAsync(chatId, rssUrl)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Broadcasting RSS Feed.");
                    Thread.Sleep(4000);
                }

                // }
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);

            Log.Information($"RSS Scheduler finished. New RSS Count: {newRssCount}");

            return newRssCount;
        }

        public static async Task ExecuteUrlAsync(long chatId, string rssUrl)
        {
            int newRssCount = 0;
            var rssService = new RssService();

            var rssFeeds = await FeedReader.ReadAsync(rssUrl)
                .ConfigureAwait(false);
            var rssTitle = rssFeeds.Title;

            // var castLimit = 10;
            // var castStep = 0;

            foreach (var rssFeed in rssFeeds.Items)
            {
                // Prevent flood in first time;
                // if (castLimit == castStep)
                // {
                //     Log.Information($"Send stopped due limit {castLimit} for prevent flooding in first time");
                //     break;
                // }

                var whereHistory = new Dictionary<string, object>()
                {
                    ["chat_id"] = chatId,
                    ["rss_source"] = rssUrl
                };

                var rssHistory = await rssService.GetRssHistory(whereHistory)
                    .ConfigureAwait(false);
                var lastRssHistory = rssHistory.LastOrDefault();

                if (!rssHistory.Any()) break;

                var lastArticleDate = DateTime.Parse(lastRssHistory.PublishDate);
                var currentArticleDate = rssFeed.PublishingDate.Value;

                if (currentArticleDate < lastArticleDate)
                {
                    Log.Information($"Current article is older than last article. Stopped.");
                    break;
                }

                Log.Information($"LastArticleDate: {lastArticleDate}");
                Log.Information($"CurrentArticleDate: {currentArticleDate}");


                Log.Information("Prepare sending article.");

                var titleLink = $"{rssTitle} - {rssFeed.Title}".MkUrl(rssFeed.Link);
                var category = rssFeed.Categories.MkJoin(", ");
                var sendText = $"{rssTitle} - {rssFeed.Title}" +
                               $"\n{rssFeed.Link}" +
                               $"\nTags: {category}";

                var where = new Dictionary<string, object>()
                {
                    {"chat_id", chatId},
                    {"url", rssFeed.Link}
                };

                var isExist = await rssService.IsExistInHistory(where)
                    .ConfigureAwait(false);
                if (isExist)
                {
                    Log.Information($"This feed has sent to {chatId}");
                    break;
                }

                Log.Information($"Sending feed to {chatId}");

                try
                {
                    await BotSettings.Client.SendTextMessageAsync(chatId, sendText, ParseMode.Html)
                        .ConfigureAwait(false);

                    var data = new Dictionary<string, object>()
                    {
                        {"url", rssFeed.Link},
                        {"rss_source", rssUrl},
                        {"chat_id", chatId},
                        {"title", rssFeed.Title},
                        {"publish_date", rssFeed.PublishingDate.ToString()},
                        {"author", rssFeed.Author},
                        {"created_at", DateTime.Now.ToString(CultureInfo.InvariantCulture)}
                    };

                    Log.Information($"Writing to RSS History");
                    await rssService.SaveRssHistoryAsync(data)
                        .ConfigureAwait(false);

                    // castStep++;
                    newRssCount++;
                }
                catch (ChatNotFoundException chatNotFoundException)
                {
                    Log.Information($"May Bot not added in {chatId}.");
                    Log.Error(chatNotFoundException, "Chat Not Found");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "RSS Broadcaster error");
                }
            }
        }

        public static async Task<string> FindUrlFeed(this string url)
        {
            Log.Information($"Scanning {url} ..");
            var urls = await FeedReader.GetFeedUrlsFromUrlAsync(url)
                .ConfigureAwait(false);
            Log.Information($"UrlFeeds: {urls.ToJson()}");

            string feedUrl = "";
            var urlCount = urls.Count();
            
            if (urlCount == 1) // no url - probably the url is already the right feed url
                feedUrl = url;
            else if (urlCount == 1)
                feedUrl = urls.First().Url;
            else if (urlCount == 2) // if 2 urls, then its usually a feed and a comments feed, so take the first per default
                feedUrl = urls.First().Url;

            return feedUrl;
        }

        public static async Task<bool> IsValidUrlFeed(this string url)
        {
            bool isValid = false;
            try
            {
                var feed = await FeedReader.ReadAsync(url)
                    .ConfigureAwait(false);
                isValid = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Validating RSS Feed");
            }

            Log.Debug($"{url} IsValidUrlFeed: {isValid}");

            return isValid;
        }
    }
}