using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;

namespace WinTenBot.Handlers
{
    class UpdateMembersList : IUpdateHandler
    {
        public Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;
            ConsoleHelper.WriteLine("Updating chat members list...");
            Console.ResetColor();

            return next(context);
        }
    }
}
