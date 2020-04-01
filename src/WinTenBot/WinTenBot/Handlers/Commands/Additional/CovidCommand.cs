using System.Threading;
using System.Threading.Tasks;
using Serilog;
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
            var txt = _telegramProvider.Message.Text;
            var partTxt = txt.SplitText(" ").ToArray();
            var part1 = partTxt.ValueOfIndex(1); // Country

            await _telegramProvider.SendTextAsync("🔍 Getting information..");

            var sendText = "";
            if (part1.IsNullOrEmpty())
            {
                Log.Information("Getting Covid info Global");
                // var sendText = await CovidHelper.GetCovidUpdatesAsync();
                sendText = await CovidHelper.GetCovidAll();
            }
            else
            {
                Log.Information($"Getting Covid info by Region: {part1}");
                sendText = await CovidHelper.GetCovidByCountry(part1);
            }

            await _telegramProvider.EditAsync(sendText);
            
        }
    }
}