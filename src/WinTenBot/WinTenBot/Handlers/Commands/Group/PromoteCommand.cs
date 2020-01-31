using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Group
{
    public class PromoteCommand : CommandBase
    {
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;
            if (msg.ReplyToMessage != null)
            {
                msg = msg.ReplyToMessage;
            }
            
            var userId = msg.From.Id;
            var nameLink = msg.GetFromNameLink();

            var sendText = $"{nameLink} berhasil menjadi admin";

            var promote = await _requestProvider.PromoteChatMemberAsync(userId);
            if (!promote.IsSuccess)
            {
                var errorCode = promote.ErrorCode;
                var errorMessage = promote.ErrorMessage;
                
                sendText = $"Promote {nameLink} gagal" +
                           $"\nPesan: {errorMessage}";
            }

            await _requestProvider.SendTextAsync(sendText);
        }
    }
}