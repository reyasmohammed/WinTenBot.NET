using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    public class MigrateCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);

            await _telegramProvider.SendTextAsync("Migrate starting..");

            Thread.Sleep(3000);

            await _telegramProvider.EditAsync("Migrate finish..");
        }
    }
}