using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers
{
    class StickerHandler : IUpdateHandler
    {
        private RequestProvider _requestProvider;
        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            
            Message msg = context.Update.Message;
            Sticker incomingSticker = msg.Sticker;

            var chat = await _requestProvider.GetChat();
            var stickerSetName = chat.StickerSetName ?? "EvilMinds";
            StickerSet evilMindsSet = await context.Bot.Client.GetStickerSetAsync(stickerSetName, cancellationToken);

            Sticker similarEvilMindSticker = evilMindsSet.Stickers.FirstOrDefault(
                sticker => incomingSticker.Emoji.Contains(sticker.Emoji)
            );

            Sticker replySticker = similarEvilMindSticker ?? evilMindsSet.Stickers.First();

            await context.Bot.Client.SendStickerAsync(
                msg.Chat,
                replySticker.FileId,
                replyToMessageId: msg.MessageId, cancellationToken: cancellationToken);
        }
    }
}
