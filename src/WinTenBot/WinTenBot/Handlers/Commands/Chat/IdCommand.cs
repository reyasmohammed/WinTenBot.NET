using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Chat
{
    public class IdCommand : CommandBase
    {
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;

            if (msg.ReplyToMessage != null)
            {
                msg = msg.ReplyToMessage;
            }

            var chatTitle = msg.Chat.Title;
            var chatId = msg.Chat.Id;
            var chatType = msg.Chat.Type;

            var userId = msg.From.Id;
            var username = msg.From.Username;
            var name = (msg.From.FirstName + " " + msg.From.LastName).Trim();
            var userLang = msg.From.LanguageCode;

            var text = $"👥 <b>{chatTitle}</b>\n" +
                       $"ID: <code>{chatId}</code>\n" +
                       $"Type: <code>{chatType}</code>\n\n" +
                       $"👤 <b>{name}</b>\n" +
                       $"ID: <code>{userId}</code>\n" +
                       $"Username: @{username}\n" +
                       $"Language: {userLang.ToUpper()}";

            await _requestProvider.SendTextAsync(text);
            //            await context.Bot.Client.SendTextMessageAsync(
            //                msg.Chat,
            //                text,
            //                ParseMode.Html,
            //                replyToMessageId: msg.MessageId
            //            );
        }
    }
}