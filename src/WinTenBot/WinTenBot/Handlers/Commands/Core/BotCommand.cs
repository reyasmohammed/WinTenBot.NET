using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    public class BotCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            var isSudoer = _telegramProvider.IsSudoer();
            if (!isSudoer) return;

            var param1 = _telegramProvider.Message.Text.Split(" ").ValueOfIndex(1);
            switch (param1)
            {
                case "migrate":
                    await _telegramProvider.SendTextAsync("Migrating ");
                    MigrationHelper.MigrateMysql();
                    MigrationHelper.MigrateSqlite();
                    await _telegramProvider.SendTextAsync("Migrate complete ");

                    break;
            }
        }
    }
}