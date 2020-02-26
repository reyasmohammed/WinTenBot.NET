using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Events
{
    public class LeftChatMemberEvent : IUpdateHandler
    {
        private ElasticSecurityService _elasticSecurityService;
        private RequestProvider _requestProvider;

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;
            _elasticSecurityService = new ElasticSecurityService(msg);
            _requestProvider = new RequestProvider(context);
            var leftMember = msg.LeftChatMember;
            var isBan = await _elasticSecurityService.IsExistInCache(leftMember.Id);

            if (!isBan)
            {
                Log.Information("Left Chat Members...");

                var chatTitle = msg.Chat.Title;

                var newMembers = msg.LeftChatMember;
                var leftFullName = newMembers.FirstName;


                // var fromName = msg.From.FirstName;

                var sendText = $"Sampai jumpa lagi {leftFullName} " +
                               $"\nKami di <b>{chatTitle}</b> menunggumu kembali.. :(";

                await _requestProvider.SendTextAsync(sendText);
            }
            else
            {
                Log.Information($"Left Message ignored because {leftMember} is Global Banned");

            }
        }
    }
}