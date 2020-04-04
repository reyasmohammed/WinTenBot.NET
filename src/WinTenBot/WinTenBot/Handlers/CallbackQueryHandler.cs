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
                case "action":
                    var callbackResult = new ActionCallback(_telegramProvider);
                    Log.Information($"ActionResult: {callbackResult.ToJson(true)}");
                    break;
                
                case "help":
                    var helpCallback = new HelpCallback(_telegramProvider);
                    Log.Information($"HelpResult: {helpCallback.ToJson(true)}");
                    break;

                case "setting":
                    var settingsCallback = new SettingsCallback(_telegramProvider);
                    Log.Information($"SettingsResult: {settingsCallback.ToJson(true)}");
                    break;
                
                case "verify":
                    var verifyCallback = new VerifyCallback(_telegramProvider);
                    Log.Information($"VerifyResult: {verifyCallback.ToJson(true)}");
                    break;

            }

            // await context.Bot.Client.AnswerCallbackQueryAsync(cq.Id, "PONG", true);

            await next(context, cancellationToken);
        }
    }
}