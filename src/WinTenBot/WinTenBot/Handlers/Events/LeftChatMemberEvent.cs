using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WinTenBot.Helpers;

namespace WinTenBot.Handlers.Events
{
    public class LeftChatMemberEvent : IUpdateHandler
    {
        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;

            ConsoleHelper.WriteLine("Left Chat Members...");

            var chatTitle = msg.Chat.Title;

            var newMembers = msg.LeftChatMember;
            var leftFullName = newMembers.FirstName;
//            var newMemberStr = new StringBuilder();
//            foreach (var newMember in newMembers)
//            {
//                var fullName = (newMember.FirstName + newMember.LastName).Trim();
//                var nameLink = MemberHelper.GetNameLink(newMember.Id, fullName);
//                if (newMembers.IndexOf(newMember) != newMembers.Length - 1)
//                {
//                    newMemberStr.Append(nameLink + ", ");
//                }
//                else
//                {
//                    newMemberStr.Append(nameLink);
//                }
//            }

            var fromName = msg.From.FirstName;

            var sendText = $"Sampai jumpa lagi {leftFullName} " +
                           $"\nKami di <b>{chatTitle}</b> menunggumu kembali.. :(";

            await context.Bot.Client.SendTextMessageAsync(
                msg.Chat,
                sendText,
                ParseMode.Html
            );
        }
    }
}