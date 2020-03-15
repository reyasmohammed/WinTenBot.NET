using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Handlers.Callbacks;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers
{
    public class CallbackQueryHandler : IUpdateHandler
    {
        private TelegramProvider _telegramProvider;

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            CallbackQuery cq = context.Update.CallbackQuery;
            _telegramProvider.CallBackMessageId = cq.Message.MessageId;

            Log.Information("CallbackQuery" + cq.ToJson(true));
            // Log.Information($"CallBackData: {cq.Data}");

            var partsCallback = cq.Data.SplitText(" ");
            Log.Information($"Callbacks: {partsCallback.ToJson()}");

            switch (partsCallback[0]) // Level 0
            {
                case "help":
                    var sendText = await partsCallback[1].LoadInBotDocs();
                    Log.Information($"Docs: {sendText}");
                    var subPartsCallback = partsCallback[1].SplitText("/");

                    Log.Information($"SubParts: {subPartsCallback.ToJson()}");
                    var jsonButton = partsCallback[1];

                    if (subPartsCallback.Count > 1)
                    {
                        jsonButton = subPartsCallback[0];

                        switch (subPartsCallback[1])
                        {
                            case "info":
                                jsonButton = subPartsCallback[1];
                                break;
                        }
                    }

                    var keyboard = await $"Storage/Buttons/{jsonButton}.json".JsonToButton();


                    await _telegramProvider.EditMessageCallback(sendText, keyboard);

                    // var a = new HelpCallbackQuery(cq);
                    break;

                case "verify":
                    var verifyCallback = new VerifyCallback(_telegramProvider);
                    Log.Information($"VerifyResult: {verifyCallback.ToJson(true)}");
                    break;

                case "setting":
                    var settingsCallback = new SettingsCallback(_telegramProvider);
                    Log.Information($"SettingsResult: {settingsCallback.ToJson(true)}");
                    break;
            }

            // await context.Bot.Client.AnswerCallbackQueryAsync(cq.Id, "PONG", true);

            await next(context);
        }
    }
}