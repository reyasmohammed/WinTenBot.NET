using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CodeHollow.FeedReader;
using Hangfire;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Services;

namespace WinTenBot.Scheduler
{
    public static class RssScheduler
    {
        public static void InitScheduler()
        {
            Task.Run(async () =>
            {
                ConsoleHelper.WriteLine("Initializing RSS Scheduler.");

                var baseId = "rss-scheduler";
                var rssService = new RssService();

                ConsoleHelper.WriteLine("Getting list Chat ID");
                var listChatId = await rssService.GetListChatIdAsync();
                foreach (DataRow row in listChatId.Rows)
                {
                    var chatId = row["chat_id"].ToString();
                    var recurringId = $"{chatId}-{baseId}";

                    ConsoleHelper.WriteLine($"Creating Jobs for {chatId}");

                    RecurringJob.RemoveIfExists(recurringId);
                    RecurringJob.AddOrUpdate(recurringId, () => RssHelper.ExecBroadcasterAsync(chatId), "*/5 * * * *");
                }

                ConsoleHelper.WriteLine("Registering RSS Scheduler complete.");
            });
        }

        [Obsolete("This function is deprecated. Please use RssHelper")]
        public static async Task ExecScheduler(string chatId)
        {
            ConsoleHelper.WriteLine("Starting RSS Scheduler.");

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
                
                var rssLimit = 3;
                var rssStep = 1;
                
                foreach (var rssFeed in rssFeeds.Items.Take(rssLimit))
                {
                    // if (rssStep == rssLimit)
                    // {
                        // ConsoleHelper.WriteLine($"RSS Limit to {rssLimit} in first time.");
                        // break;
                    // }

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
                                {"publish_date", rssFeed.PublishingDate.ToString()},
                                {"author", rssFeed.Author}
                            };

                            ConsoleHelper.WriteLine($"Writing to RSS History");
                            await rssService.SaveRssAsync(data);

                            rssStep++;
                        }
                        catch (ChatNotFoundException chatNotFoundException)
                        {
                            ConsoleHelper.WriteLine($"May Bot not added in {chatId}." +
                                                    $"\nMessage: {chatNotFoundException.Message}");
                        }
                    }
                    else
                    {
                        ConsoleHelper.WriteLine($"This feed has sent to {chatId}");
                    }
                }
            }

            ConsoleHelper.WriteLine("RSS Scheduler finished.");
        }
    }
}