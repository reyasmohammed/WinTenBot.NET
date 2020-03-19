using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Additional
{
    public class CovidCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);

            await _telegramProvider.SendTextAsync("Sedang mendapatkan informasi..");

            var sendText = await CovidHelper.GetCovidUpdatesAsync();

            await _telegramProvider.EditAsync(sendText);
        }
    }
}