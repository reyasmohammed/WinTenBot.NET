using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers
{
    public class GenericMessageHandler : IUpdateHandler
    {
        private CasBanProvider _casBanProvider;

        public GenericMessageHandler()
        {
            _casBanProvider = new CasBanProvider();
        }
        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;
            ConsoleHelper.WriteLine(msg.ToJson());
            
            await _casBanProvider.IsCasBan(msg.From.Id);
            
            //            await context.Bot.Client.SendTextMessageAsync(
            //                msg.Chat, "You said:\n" + msg.Text
            //            );

            await next(context);
        }
    }
}