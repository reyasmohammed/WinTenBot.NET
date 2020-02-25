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
        private RequestProvider _requestProvider;

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            CallbackQuery cq = context.Update.CallbackQuery;
            _requestProvider.CallBackMessageId = cq.Message.MessageId;

            Log.Information(cq.ToJson());
            Log.Information($"CallBackData: {cq.Data}");

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
                    

                    await _requestProvider.EditMessageCallback(sendText, keyboard);

                    // var a = new HelpCallbackQuery(cq);
                    break;

                case "verification":
                    break;
            }

            // await context.Bot.Client.AnswerCallbackQueryAsync(cq.Id, "PONG", true);

            await next(context);
        }
    }
}