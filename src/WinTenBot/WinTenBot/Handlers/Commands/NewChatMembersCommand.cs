using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace WinTenBot.Handlers.Commands
{
    public class NewChatMembersCommand : IUpdateHandler
    {
        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;

            Console.WriteLine("New Chat Members...");

            var chatTitle = msg.Chat.Title;

            var newMembers = msg.NewChatMembers;

            var fromName = msg.From.FirstName;

            var sendText = $"Hai {fromName}, selamat datang di kontrakan {chatTitle}" +
                $"\nCount: {newMembers.Length}";

            await context.Bot.Client.SendTextMessageAsync(
                msg.Chat,
                sendText,
                ParseMode.Html,
                replyToMessageId: msg.MessageId
            );
        }
    }
}