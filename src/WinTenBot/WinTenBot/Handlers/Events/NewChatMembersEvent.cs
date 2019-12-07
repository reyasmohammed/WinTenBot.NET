using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Events
{
    public class NewChatMembersEvent : IUpdateHandler
    {
        private CasBanProvider _casBanProvider;

        public NewChatMembersEvent()
        {
            _casBanProvider = new CasBanProvider();
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;

            ConsoleHelper.WriteLine("New Chat Members...");

            var chatTitle = msg.Chat.Title;

            var newMembers = msg.NewChatMembers;
            var newMemberStr = new StringBuilder();
            foreach (var newMember in newMembers)
            {
                var isCasBan = await _casBanProvider.IsCasBan(newMember.Id);

                var fullName = (newMember.FirstName + " " + newMember.LastName).Trim();
                var nameLink = MemberHelper.GetNameLink(newMember.Id, fullName);
                if (newMembers.IndexOf(newMember) != newMembers.Length - 1)
                {
                    newMemberStr.Append(nameLink + ", ");
                }
                else
                {
                    newMemberStr.Append(nameLink);
                }
            }

            var fromName = msg.From.FirstName;

            var sendText = $"Hai {newMemberStr}" +
                           $"\nSelamat datang di kontrakan <b>{chatTitle}</b>" +
                           $"\nCount: {newMembers.Length}";

            await context.Bot.Client.SendTextMessageAsync(
                msg.Chat,
                sendText,
                ParseMode.Html
            );
        }
    }
}