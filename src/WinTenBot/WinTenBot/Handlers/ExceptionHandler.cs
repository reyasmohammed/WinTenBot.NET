using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;

namespace WinTenBot.Handlers
{
    public class ExceptionHandler : IUpdateHandler
    {
        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            var u = context.Update;

            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                Log.Error(e,"Exception Handler");
                // Console.BackgroundColor = ConsoleColor.Black;
                // Console.ForegroundColor = ConsoleColor.Red;
                Log.Error("An error occured in handling update {0}.{1}{2}", u.Id, Environment.NewLine, e);
                // Console.ResetColor();
            }
        }
    }
}