using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Core
{
    public class MigrateCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);

            await _telegramService.SendTextAsync("Migrate starting..")
                .ConfigureAwait(false);

            Thread.Sleep(3000);

            await _telegramService.EditAsync("Migrate finish..")
                .ConfigureAwait(false);
        }
    }
}