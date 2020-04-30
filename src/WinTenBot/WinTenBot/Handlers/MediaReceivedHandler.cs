using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers
{
    public class MediaReceivedHandler : IUpdateHandler
    {
        private MediaFilterService _mediaFilterService;
        private TelegramProvider _telegramProvider;

        public MediaReceivedHandler()
        {
            _mediaFilterService = new MediaFilterService();
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            var msg = context.Update.Message;

            Log.Information("Media received... ");
            var fileId = msg.GetFileId();
            var reducedFileId = msg.GetReducedFileId();

            // var isBan = await _mediaFilterService.IsExistInCache("file_id", msg.GetReducedFileId());
            var isBan = await _mediaFilterService.IsExist("file_id", fileId);
            if (isBan)
            {
                await _telegramProvider.DeleteAsync(msg.MessageId);
            }

            Log.Information($"Media isBan: {isBan}");

            if (BotSettings.HostingEnvironment.IsProduction())
                await _mediaFilterService.UpdateCacheAsync();
            else
                Log.Information($"Update cache skipped because local Env");

            Log.Information("Media Filter complete.");

            await next(context);
        }
    }
}