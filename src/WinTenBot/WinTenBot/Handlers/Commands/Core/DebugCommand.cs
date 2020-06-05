using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Common;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Core
{
    public class DebugCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);

            var msg = context.Update.Message;
            var json = msg.ToJson(true);

            Log.Information(json.Length.ToString());

            var sendText = $"Debug:\n {json}";
            await _telegramService.SendTextAsync(sendText);
        }
    }
}