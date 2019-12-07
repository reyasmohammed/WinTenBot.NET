using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Handlers.Callbacks;
using WinTenBot.Helpers;

namespace WinTenBot.Handlers
{
    public class CallbackQueryHandler : IUpdateHandler
    {
        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            CallbackQuery cq = context.Update.CallbackQuery;
            ConsoleHelper.WriteLine(cq.ToJson());
            ConsoleHelper.WriteLine(cq.Data);
            
            var partsCallback = cq.Data.SplitText(" ");
            ConsoleHelper.WriteLine($"Callbacks: {partsCallback.ToJson()}");

            switch (partsCallback[0]) // Level 0
            {
                case "help":
                    var a = new HelpCallbackQuery(cq);
                    break;
                    
                case "verification":
                    break;
            }

            await context.Bot.Client.AnswerCallbackQueryAsync(cq.Id, "PONG", true);

            await next(context);
        }
    }
}