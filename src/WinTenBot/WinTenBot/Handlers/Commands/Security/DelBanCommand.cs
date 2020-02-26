using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Security
{
    public class DelBanCommand : CommandBase
    {
        private RequestProvider _requestProvider;
        private ElasticSecurityService _elasticSecurityService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            var msg = context.Update.Message;
            var chatId = msg.Chat.Id;
            var fromId = msg.From.Id;
            var partedText = msg.Text.Split(" ");
            var param1 = partedText.ValueOfIndex(1); // User ID

            _requestProvider = new RequestProvider(context);
            _elasticSecurityService = new ElasticSecurityService(msg);

            if (fromId.IsSudoer())
            {
                // if (msg.ReplyToMessage != null)
                // {
                    var repMsg = msg.ReplyToMessage;
                    var userId = param1.ToInt();

                    Log.Information("Execute Global DelBan");
                    await _requestProvider.SendTextAsync("Mempersiapkan..");
                    await _requestProvider.DeleteAsync(msg.MessageId);

                    var isBan = await _elasticSecurityService.IsExist(userId);
                    Log.Information($"IsBan: {isBan}");
                    if (isBan)
                    {
                        await _requestProvider.EditAsync("Memperbarui informasi..");
                        var save = await _elasticSecurityService.DeleteBanAsync(userId);
                        Log.Information($"SaveBan: {save}");

                        await _requestProvider.EditAsync("Menulis ke Cache..");
                        await _elasticSecurityService.UpdateCacheAsync();

                        await _requestProvider.EditAsync("Misi berhasil.");
                    }
                    else
                    {
                        await _requestProvider.EditAsync("Pengguna tidak di ban");
                    }

                // }
                // else
                // {
                //     await _chatProcessor.SendAsync("Balas seseorang yang mau di ban");
                // }
            }
            else
            {
                await _requestProvider.SendTextAsync("Unauthorized");
            }

            await _requestProvider.DeleteAsync(delay: 3000);
        }
    }
}