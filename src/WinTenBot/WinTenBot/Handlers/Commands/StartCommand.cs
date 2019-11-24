using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;

namespace WinTenBot.Handlers.Commands
{
    class StartCommand : CommandBase
    {
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            await context.Bot.Client.SendTextMessageAsync(context.Update.Message.Chat, "Hello, World!");
            await next(context);
        }
    }
}
