using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Handlers.Callbacks;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers
{
    public class CallbackQueryHandler : IUpdateHandler
    {
        private ChatProcessor _chatProcessor;

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            CallbackQuery cq = context.Update.CallbackQuery;
            _chatProcessor.CallBackMessageId = cq.Message.MessageId;

            ConsoleHelper.WriteLine(cq.ToJson());
            ConsoleHelper.WriteLine($"CallBackData: {cq.Data}");

            var partsCallback = cq.Data.SplitText(" ");
            ConsoleHelper.WriteLine($"Callbacks: {partsCallback.ToJson()}");

            switch (partsCallback[0]) // Level 0
            {
                case "help":
                    var sendText = await partsCallback[1].LoadInBotDocs();
                    ConsoleHelper.WriteLine($"Docs: {sendText}");
                    var subPartsCallback = partsCallback[1].SplitText("/");

                    ConsoleHelper.WriteLine($"SubParts: {subPartsCallback.ToJson()}");
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
                    

                    await _chatProcessor.EditMessageCallback(sendText, keyboard);

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