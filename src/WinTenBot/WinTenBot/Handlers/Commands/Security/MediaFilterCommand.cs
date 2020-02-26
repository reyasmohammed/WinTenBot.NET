using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Security
{
    public class MediaFilterCommand : CommandBase
    {
        private RequestProvider _requestProvider;
        private MediaFilterService _mediaFilterService;

        public MediaFilterCommand()
        {
            _mediaFilterService = new MediaFilterService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;

            var sendText = "Saat ini hanya untuk Sudoer saja.";
            if (msg.From.Id.IsSudoer())
            {
                sendText = "Reply pesan untuk menyaring..";
                if (msg.ReplyToMessage != null)
                {
                    var repMsg = msg.ReplyToMessage;
                    Log.Information(msg.Type.ToJson(true));

                    var fileId = repMsg.GetReducedFileId();

                    var isExist = await _mediaFilterService.IsExist("file_id", fileId);
                    if (!isExist)
                    {
                        var data = new Dictionary<string, object>()
                        {
                            {"file_id", fileId},
                            {"type_data", repMsg.Type.ToString().ToLower()},
                            {"blocked_by", msg.From.Id},
                            {"blocked_from", msg.Chat.Id}
                        };

                        await _mediaFilterService.SaveAsync(data);
                        sendText = "File ini berhasil di simpan";
                    }
                    else
                    {
                        sendText = "File ini sudah di simpan";
                    }
                }
            }
            else
            {
                sendText =
                    "Fitur ini membutuhkan akses Sudoer, namun file yang Anda laporkan sudah di teruskan ke Team, " +
                    "terima kasih atas laporan nya.";
            }

            await _requestProvider.SendTextAsync(sendText);
        }
    }
}