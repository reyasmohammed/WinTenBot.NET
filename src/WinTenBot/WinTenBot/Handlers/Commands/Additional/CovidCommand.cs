using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Model;
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

            await _telegramProvider.SendTextAsync("Sedang mendapatkan..");
            
            var urlApi = "https://coronavirus-tracker-api.herokuapp.com/all";
            var covidAll = await urlApi.GetJsonAsync<CovidAll>(cancellationToken: cancellationToken);

            await covidAll.WriteCacheAsync("covid-all.json");

            // Log.Information($"CovidAll: {covidAll.ToJson(true)}");

            var confirmed = covidAll.Confirmed;
            var deaths = covidAll.Deaths;
            var recovered = covidAll.Recovered;
            
            var messageBuild = new StringBuilder();

            messageBuild.AppendLine("Corona Updates.");
            messageBuild.AppendLine("1. Confirmed");
            messageBuild.AppendLine($"LastUpdate: {confirmed.LastUpdated}");
            messageBuild.AppendLine($"Latest: {confirmed.Latest}");
            messageBuild.AppendLine();

            messageBuild.AppendLine("2. Deaths");
            messageBuild.AppendLine($"LastUpdate: {deaths.LastUpdated}");
            messageBuild.AppendLine($"Latest: {deaths.Latest}");
            messageBuild.AppendLine();

            messageBuild.AppendLine("3. Recovered");
            messageBuild.AppendLine($"LastUpdate: {recovered.LastUpdated}");
            messageBuild.AppendLine($"Latest: {recovered.Latest}");
            messageBuild.AppendLine();

            messageBuild.AppendLine($"Source: {confirmed.Source}");

            await _telegramProvider.EditAsync(messageBuild.ToString());
        }
    }
}