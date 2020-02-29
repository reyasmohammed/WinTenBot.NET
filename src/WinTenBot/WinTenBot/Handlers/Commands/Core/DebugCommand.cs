using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    public class DebugCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);

            var msg = context.Update.Message;
            var json = msg.ToJson(true);

            Log.Information(json.Length.ToString());

            var sendText = $"Debug:\n {json}";
            await _telegramProvider.SendTextAsync(sendText);
        }
    }
}