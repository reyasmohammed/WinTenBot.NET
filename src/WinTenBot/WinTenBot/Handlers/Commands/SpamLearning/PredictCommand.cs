using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.SpamLearning
{
    public class PredictCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            var message = _telegramService.Message;

            if (message.ReplyToMessage != null)
            {
                var repMsg = message.ReplyToMessage;
                var repMsgText = repMsg.Text;

                await _telegramService.SendTextAsync("Sedang memprediksi pesan")
                    .ConfigureAwait(false);

                var isSpam = LearningHelper.PredictMessage(repMsgText);
                await _telegramService.EditAsync($"IsSpam: {isSpam}").ConfigureAwait(false);

                return;
            }
            else
            {
                await _telegramService.SendTextAsync("Predicting message").ConfigureAwait(false);
            }
        }
    }
}