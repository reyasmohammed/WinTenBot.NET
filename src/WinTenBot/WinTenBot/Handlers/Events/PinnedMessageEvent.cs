using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers.Events
{
    public class PinnedMessageEvent : IUpdateHandler
    {
        private ChatProcessor _chatProcessor;

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            var msg = context.Update.Message;

            var pinnedMsg = msg.PinnedMessage;
            var sendText = $"📌 Pesan di sematkan baru!" +
                           $"\nPengirim: {pinnedMsg.GetFromNameLink()}" +
                           $"\nPengepin: {msg.GetFromNameLink()}";

            await _chatProcessor.SendAsync(sendText);
        }
    }
}