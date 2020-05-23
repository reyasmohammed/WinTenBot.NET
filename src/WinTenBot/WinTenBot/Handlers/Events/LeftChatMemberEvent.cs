using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Events
{
    public class LeftChatMemberEvent : IUpdateHandler
    {
        private TelegramService _telegramService;

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;
            _telegramService = new TelegramService(context);
            var leftMember = msg.LeftChatMember;
            var leftUserId = leftMember.Id;
            var isBan = await leftUserId.CheckGBan();

            if (!isBan)
            {
                Log.Information("Left Chat Members...");

                var chatTitle = msg.Chat.Title;
                var leftChatMember = msg.LeftChatMember;
                var leftFullName = leftChatMember.FirstName;

                var sendText = $"Sampai jumpa lagi {leftFullName} " +
                               $"\nKami di <b>{chatTitle}</b> menunggumu kembali.. :(";

                await _telegramService.SendTextAsync(sendText);
            }
            else
            {
                Log.Information($"Left Message ignored because {leftMember} is Global Banned");
            }
        }
    }
}