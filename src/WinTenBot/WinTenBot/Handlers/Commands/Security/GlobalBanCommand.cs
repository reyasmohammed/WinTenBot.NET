using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;

namespace WinTenBot.Handlers.Commands.Security
{
    public class GlobalBanCommand:CommandBase
    {
        public override Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}