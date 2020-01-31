using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Security
{
    public class GlobalBanCommand : CommandBase
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
            var param1 = partedText.ValueOfIndex(1);

            _requestProvider = new RequestProvider(context);
            _elasticSecurityService = new ElasticSecurityService(msg);

            if (fromId.IsSudoer())
            {
                switch (param1)
                {
                    case "sync":
                        await _requestProvider.SendTextAsync("Memperbarui cache..");
                        await _elasticSecurityService.UpdateCacheAsync();
                        await _requestProvider.EditAsync("Selesai memperbarui..");

                        break;

                    default:
                        if (msg.ReplyToMessage != null)
                        {
                            var repMsg = msg.ReplyToMessage;
                            var userId = repMsg.From.Id;
                    
                            ConsoleHelper.WriteLine("Execute Global Ban");
                            await _requestProvider.SendTextAsync("Mempersiapkan..");
                            await _requestProvider.DeleteAsync(msg.MessageId);

                            var isBan = await _elasticSecurityService.IsExist(userId);
                            ConsoleHelper.WriteLine($"IsBan: {isBan}");
                            if (isBan)
                            {
                                await _requestProvider.EditAsync("Pengguna sudah di ban");
                            }
                            else
                            {
                                var data = new Dictionary<string, object>()
                                {
                                    {"user_id",userId},
                                    {"banned_by", fromId},
                                    {"banned_from", chatId}
                                };

                                await _requestProvider.EditAsync("Menyimpan informasi..");
                                var save = await _elasticSecurityService.SaveBanAsync(data);
                                ConsoleHelper.WriteLine($"SaveBan: {save}");

                                await _requestProvider.EditAsync("Menulis ke Cache..");
                                await _elasticSecurityService.UpdateCacheAsync();

                                await _requestProvider.EditAsync("Misi berhasil.");
                            }
                    
                        }
                        else
                        {
                            await _requestProvider.SendTextAsync("Balas seseorang yang mau di ban");

                        }
                        break;
                }
            }
            else
            {
                await _requestProvider.SendTextAsync("Unauthorized");
            }
            
            await _requestProvider.DeleteAsync(delay:3000);
        }
    }
}