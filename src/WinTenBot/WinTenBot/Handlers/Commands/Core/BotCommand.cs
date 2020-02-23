using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    public class BotCommand:CommandBase
    {
        private RequestProvider _requestProvider;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var isSudoer = _requestProvider.IsSudoer();
            if (!isSudoer) return;

            var param1 = _requestProvider.Message.Text.Split(" ").ValueOfIndex(1);
            switch (param1)
            {
                case "migrate":
                    await _requestProvider.SendTextAsync("Migrating ");
                    MigrationHelper.MigrateMysql();
                    MigrationHelper.MigrateSqlite();
                    await _requestProvider.SendTextAsync("Migrate complete ");

                    break;
                
            }
        }
    }
}
