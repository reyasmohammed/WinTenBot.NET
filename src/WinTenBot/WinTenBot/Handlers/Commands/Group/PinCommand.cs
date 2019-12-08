using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers.Commands
{
    public class PinCommand : CommandBase
    {
        private ChatProcessor _chatProcessor;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            var msg = context.Update.Message;
            var client = context.Bot.Client;

            var sendText = "Balas pesan yang akan di pin";

            if (msg.ReplyToMessage != null)
            {
                var pin = client.PinChatMessageAsync(
                    msg.Chat.Id,
                    msg.ReplyToMessage.MessageId);
//                ConsoleHelper.WriteLine(pin.);
            }

            await _chatProcessor.SendAsync(sendText);
//            throw new System.NotImplementedException();
        }
    }
}