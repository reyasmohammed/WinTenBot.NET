using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Services;

namespace WinTenBot.Handlers
{
    public class CallTagsReceivedHandler : IUpdateHandler
    {
        private ChatProcessor chatProcessor;
        private TagsService tagsService;

        public CallTagsReceivedHandler()
        {
            tagsService = new TagsService();
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            chatProcessor = new ChatProcessor(context);
            Message msg = context.Update.Message;
            ConsoleHelper.WriteLine("Tags Received..");
            var partsText = msg.Text.Split(new char[] { ' ', '\n', ',' })
                .Where(x => x.Contains("#"));

            var limitedTags = partsText.Take(5);

            //            int count = 1;
            foreach (var split in limitedTags)
            {
                ConsoleHelper.WriteLine("Processing : " + split.TrimStart('#'));
                //                await chatProcessor.SendAsync($"This is tag of {split}");
                var tagData = tagsService.GetTagByTag(msg.Chat.Id, split.TrimStart('#'));
                var json = tagData.Result.ToJson();
                Console.WriteLine(json);

                var content = tagData.Result.Rows[0]["content"].ToString();
                var buttonStr = tagData.Result.Rows[0]["btn_data"].ToString();

                IReplyMarkup buttonMarkup = null;
                if (buttonStr != "")
                {
                    buttonMarkup = buttonStr.ToReplyMarkup(2);
                }

                await chatProcessor.SendAsync(content, buttonMarkup);
            }

            if (partsText.Count() > limitedTags.Count())
            {
                await chatProcessor.SendAsync("Due performance reason, we limit 5 batch call tags");
            }

            //            await context.Bot.Client.SendTextMessageAsync(
            //                msg.Chat, "You said:\n" + msg.Text
            //            );

            //            await next(context);
        }
    }
}