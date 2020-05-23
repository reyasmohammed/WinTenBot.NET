using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Core
{
    public class TestCommand : CommandBase
    {
        private RssService _rssService;
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            _rssService = new RssService(context.Update.Message);

            var chatId = _telegramService.Message.Chat.Id;
            var fromId = _telegramService.Message.From.Id;

            if (fromId.IsSudoer())
            {
                Log.Information("Test started..");
                await _telegramService.SendTextAsync("Sedang mengetes sesuatu");

                // var data = await new Query("rss_history")
                //     .Where("chat_id", chatId)
                //     .ExecForMysql()
                //     .GetAsync();
                //
                // var rssHistories = data
                //     .ToJson()
                //     .MapObject<List<RssHistory>>();
                //
                // ConsoleHelper.WriteLine(data.GetType());
                // // ConsoleHelper.WriteLine(data.ToJson(true));
                //
                // ConsoleHelper.WriteLine(rssHistories.GetType());
                // // ConsoleHelper.WriteLine(rssHistories.ToJson(true));
                //
                // ConsoleHelper.WriteLine("Test completed..");

                // await "This test".LogToChannel();

                // await RssHelper.SyncRssHistoryToCloud();
                // await BotHelper.ClearLog();

                // await SyncHelper.SyncGBanToLocalAsync();
                // var greet = TimeHelper.GetTimeGreet();

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // new[]
                    // {
                        // InlineKeyboardButton.WithCallbackData("Warn Username Limit", "info warn-username-limit"),
                        // InlineKeyboardButton.WithCallbackData("-", "callback-set warn_username_limit 3"),
                        // InlineKeyboardButton.WithCallbackData("4", "info setelah"),
                        // InlineKeyboardButton.WithCallbackData("+", "callback-set warn_username_limit 5")
                    // },
                    new[]
                    {
                        // InlineKeyboardButton.WithCallbackData("Warn Username Limit", "info warn-username-limit"),
                        InlineKeyboardButton.WithCallbackData("-", "callback-set warn_username_limit 3"),
                        InlineKeyboardButton.WithCallbackData("4", "info setelah"),
                        InlineKeyboardButton.WithCallbackData("+", "callback-set warn_username_limit 5")
                    }
                });

                await _telegramService.EditAsync("Warn Username Limit", inlineKeyboard);
            }

            // else
            // {
            //     await _requestProvider.SendTextAsync("Unauthorized.");
            // }
        }
    }
}