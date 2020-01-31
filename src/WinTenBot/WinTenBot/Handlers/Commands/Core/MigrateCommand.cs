using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers.Processors;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    public class MigrateCommand : CommandBase
    {
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);

            await _requestProvider.SendTextAsync("Migrate starting..");

            Thread.Sleep(3000);

            await _requestProvider.EditAsync("Migrate finish..");
        }
    }
}