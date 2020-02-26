using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers
{
    [Obsolete("Soon this Class will move to MessageHelper as Function.")]
    public class CallTagsReceivedHandler : IUpdateHandler
    {
        private RequestProvider _requestProvider;
        private TagsService tagsService;

        public CallTagsReceivedHandler()
        {
            tagsService = new TagsService();
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);

            Message msg = context.Update.Message;
            Log.Information("Tags Received..");
            var partsText = msg.Text.Split(new char[] { ' ', '\n', ',' })
                .Where(x => x.Contains("#"));

            var limitedTags = partsText.Take(5);

            //            int count = 1;
            foreach (var split in limitedTags)
            {
                Log.Information("Processing : " + split.TrimStart('#'));
                //                await chatProcessor.SendAsync($"This is tag of {split}");
                var tagData = await tagsService.GetTagByTag(msg.Chat.Id, split.TrimStart('#'));
                var json = tagData.ToJson();
                Log.Information(json);

                // var content = tagData.Result.Rows[0]["content"].ToString();
                // var buttonStr = tagData.Result.Rows[0]["btn_data"].ToString();

                var content = tagData[0].Content;
                var buttonStr = tagData[0].BtnData;

                InlineKeyboardMarkup buttonMarkup = null;
                if (buttonStr != "")
                {
                    buttonMarkup = buttonStr.ToReplyMarkup(2);
                }

                await _requestProvider.SendTextAsync(content, buttonMarkup);
            }

            if (partsText.Count() > limitedTags.Count())
            {
                await _requestProvider.SendTextAsync("Due performance reason, we limit 5 batch call tags");
            }
        }
    }
}