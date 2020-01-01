﻿using System.Collections.Generic;
using System.Data;
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
        public static async Task<int> ExecSchedulerAsync(string chatId)
        {
            ConsoleHelper.WriteLine("Starting RSS Scheduler.");
            int newRssCount = 0;

            var rssService = new RssService();

            ConsoleHelper.WriteLine("Getting RSS settings..");
            var rssSettings = await rssService.GetRssSettingsAsync(chatId);
            foreach (DataRow rssSetting in rssSettings.Rows)
            {
                var rssUrl = rssSetting["url_feed"].ToString();
                // var chatId = rssSetting["chat_id"].ToString();

                ConsoleHelper.WriteLine($"Processing {rssUrl} for {chatId}.");
                var rssFeeds = await FeedReader.ReadAsync(rssUrl);
                var rssTitle = rssFeeds.Title;

                foreach (var rssFeed in rssFeeds.Items)
                {
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
                                {"chat_id", chatId},
                                {"title", rssFeed.Title},
                                {"publish_date", rssFeed.PublishingDate.ToString()},
                                {"author", rssFeed.Author}
                            };

                            ConsoleHelper.WriteLine($"Writing to RSS History");
                            await rssService.SaveRssAsync(data);
                        }
                        catch (ChatNotFoundException chatNotFoundException)
                        {
                            ConsoleHelper.WriteLine($"May Bot not added in {chatId}." +
                                                    $"\n{chatNotFoundException.Message}");
                        }

                        newRssCount++;
                    }
                    else
                    {
                        ConsoleHelper.WriteLine($"This feed has sent to {chatId}");
                    }
                }
            }

            ConsoleHelper.WriteLine("RSS Scheduler finished.");
            
            return newRssCount;
        }
    }
}