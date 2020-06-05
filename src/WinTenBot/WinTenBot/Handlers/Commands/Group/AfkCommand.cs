using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Services;
using WinTenBot.Telegram;

namespace WinTenBot.Handlers.Commands.Group
{
    public class AfkCommand : CommandBase
    {
        private AfkService _afkService;
        private TelegramService _telegramService;

        public AfkCommand()
        {
            _afkService = new AfkService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            var msg = context.Update.Message;

            var data = new Dictionary<string, object>()
            {
                {"user_id", msg.From.Id},
                {"chat_id", msg.Chat.Id},
                {"is_afk", 1}
            };

            var sendText = $"{msg.GetFromNameLink()} Sedang afk.";

            if (msg.Text.GetTextWithoutCmd() != "")
            {
                var afkReason = msg.Text.GetTextWithoutCmd();
                data.Add("afk_reason", afkReason);

                sendText += $"\n<i>{afkReason}</i>";
            }

            await _telegramService.SendTextAsync(sendText);
            await _afkService.SaveAsync(data);
            await _afkService.UpdateCacheAsync();
        }
    }
}