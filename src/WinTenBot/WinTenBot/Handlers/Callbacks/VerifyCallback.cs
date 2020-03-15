using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Callbacks
{
    public class VerifyCallback
    {
        private TelegramProvider Telegram { get; set; }
        private CallbackQuery CallbackQuery { get; set; }

        public VerifyCallback(TelegramProvider telegramProvider)
        {
            Telegram = telegramProvider;
            CallbackQuery = telegramProvider.Context.Update.CallbackQuery;
            Log.Information("Receiving Verify Callback");

            Parallel.Invoke(async () => await ExecuteVerifyAsync());
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
                await Telegram.RestrictMemberAsync(userId,true);
                answer = "Terima kasih sudah verifikasi!";
            }
            
            await Telegram.AnswerCallbackQueryAsync(answer);
        }
    }
}