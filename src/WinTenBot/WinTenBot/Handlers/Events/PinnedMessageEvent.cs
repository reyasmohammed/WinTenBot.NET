using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Events
{
    public class PinnedMessageEvent : IUpdateHandler
    {
        private RequestProvider _requestProvider;

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;

            var pinnedMsg = msg.PinnedMessage;
            var sendText = $"📌 Pesan di sematkan baru!" +
                           $"\nPengirim: {pinnedMsg.GetFromNameLink()}" +
                           $"\nPengepin: {msg.GetFromNameLink()}";

            await _requestProvider.SendTextAsync(sendText);
        }
    }
}