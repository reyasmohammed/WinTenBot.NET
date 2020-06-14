﻿using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Common;
using WinTenBot.Handlers.Callbacks;
using WinTenBot.Services;

namespace WinTenBot.Handlers
{
    public class CallbackQueryHandler : IUpdateHandler
    {
        private TelegramService _telegramService;

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            CallbackQuery cq = context.Update.CallbackQuery;
            _telegramService.CallBackMessageId = cq.Message.MessageId;

            Log.Information("CallbackQuery" + cq.ToJson(true));
            // Log.Information($"CallBackData: {cq.Data}");

            var partsCallback = cq.Data.SplitText(" ");
            Log.Information($"Callbacks: {partsCallback.ToJson()}");

            switch (partsCallback[0]) // Level 0
            {
                case "action":
                    var callbackResult = new ActionCallback(_telegramService);
                    Log.Information($"ActionResult: {callbackResult.ToJson(true)}");
                    break;
                
                case "help":
                    var helpCallback = new HelpCallback(_telegramService);
                    Log.Information($"HelpResult: {helpCallback.ToJson(true)}");
                    break;

                case "setting":
                    var settingsCallback = new SettingsCallback(_telegramService);
                    Log.Information($"SettingsResult: {settingsCallback.ToJson(true)}");
                    break;
                
                case "verify":
                    var verifyCallback = new VerifyCallback(_telegramService);
                    Log.Information($"VerifyResult: {verifyCallback.ToJson(true)}");
                    break;

            }

            // await context.Bot.Client.AnswerCallbackQueryAsync(cq.Id, "PONG", true);

            await next(context, cancellationToken);
        }
    }
}