using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Services;

namespace WinTenBot.Handlers
{
    public class MediaReceivedHandler : IUpdateHandler
    {
        private MediaFilterService _mediaFilterService;
        private ChatProcessor _chatProcessor;

        public MediaReceivedHandler()
        {
            _mediaFilterService = new MediaFilterService();
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            var msg = context.Update.Message;

            ConsoleHelper.WriteLine("Media received... ");
            var isBan = await _mediaFilterService.IsExistInCache("file_id", msg.GetReducedFileId());
//            var isBan = await _mediaFilterService.IsExist("file_id", msg.GetReducedFileId());
            if (isBan)
            {
                await _chatProcessor.DeleteAsync(msg.MessageId);
            }

            ConsoleHelper.WriteLine($"Media isBan: {isBan}");
//            await _mediaFilterService.UpdateCacheAsync();
//            var cache = await _mediaFilterService.ReadCacheAsync();
        }
    }
}