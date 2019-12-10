using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Model;
using WinTenBot.Providers;

namespace WinTenBot.Handlers
{
    public class GenericMessageHandler : IUpdateHandler
    {
        private CasBanProvider _casBanProvider;
        private ChatProcessor _chatProcessor;

        public GenericMessageHandler()
        {
            _casBanProvider = new CasBanProvider();
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            Message msg = context.Update.Message;

            ConsoleHelper.WriteLine(msg.ToJson());
            ConsoleHelper.WriteLine(msg.Text);

            if (Bot.HostingEnvironment.IsProduction())
            {
                await _casBanProvider.IsCasBan(msg.From.Id);
            }

            //            await context.Bot.Client.SendTextMessageAsync(
            //                msg.Chat, "You said:\n" + msg.Text
            //            );

            await next(context);
        }
    }
}