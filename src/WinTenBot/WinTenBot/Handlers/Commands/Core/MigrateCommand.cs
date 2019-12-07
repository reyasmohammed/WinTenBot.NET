using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers.Commands.Core
{
    public class MigrateCommand : CommandBase
    {
        private ChatProcessor chatProcessor;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            chatProcessor = new ChatProcessor(context);

            await chatProcessor.SendAsync("Migrate starting..");

            Thread.Sleep(3000);

            await chatProcessor.EditAsync("Migrate finish..");
        }
    }
}