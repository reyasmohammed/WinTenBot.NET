using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers
{
    public class MediaReceivedHandler : IUpdateHandler
    {
        private RequestProvider _requestProvider;
        private MediaFilterService _mediaFilterService;

        public MediaReceivedHandler()
        {
            _mediaFilterService = new MediaFilterService();
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;

            ConsoleHelper.WriteLine("Media received... ");
            var isBan = await _mediaFilterService.IsExistInCache("file_id", msg.GetReducedFileId());
//            var isBan = await _mediaFilterService.IsExist("file_id", msg.GetReducedFileId());
            if (isBan)
            {
                await _requestProvider.DeleteAsync(msg.MessageId);
            }

            ConsoleHelper.WriteLine($"Media isBan: {isBan}");

            if (Bot.HostingEnvironment.IsProduction())
                await _mediaFilterService.UpdateCacheAsync();
            else
                ConsoleHelper.WriteLine($"Update cache skipped because local Env");

            ConsoleHelper.WriteLine("Media Filter complete.");

            await next(context);
        }
    }
}