using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Types;
using WinTenBot.Common;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Callbacks
{
    public class VerifyCallback
    {
        private TelegramService Telegram { get; set; }
        private CallbackQuery CallbackQuery { get; set; }

        public VerifyCallback(TelegramService telegramService)
        {
            Telegram = telegramService;
            CallbackQuery = telegramService.Context.Update.CallbackQuery;
            Log.Information("Receiving Verify Callback");

            Parallel.Invoke(async () =>
                await ExecuteVerifyAsync().ConfigureAwait(false));
        }

        private async Task ExecuteVerifyAsync()
        {
            var callbackData = CallbackQuery.Data;
            var fromId = CallbackQuery.From.Id;

            Log.Information($"CallbackData: {callbackData} from {fromId}");

            var partCallbackData = callbackData.Split(" ");
            var userId = partCallbackData.ValueOfIndex(1).ToInt();
            var answer = "Tombol ini bukan untukmu Bep!";

            Log.Information($"Verify UserId: {userId}");
            if (fromId == userId)
            {
                await Telegram.RestrictMemberAsync(userId, true)
                    .ConfigureAwait(false);
                answer = "Terima kasih sudah verifikasi!";
            }

            await Telegram.AnswerCallbackQueryAsync(answer)
                .ConfigureAwait(false);
        }
    }
}