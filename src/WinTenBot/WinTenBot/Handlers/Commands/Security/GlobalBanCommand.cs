using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Services;
using WinTenBot.Telegram;
using WinTenBot.Text;
using WinTenBot.Tools;

namespace WinTenBot.Handlers.Commands.Security
{
    public class GlobalBanCommand : CommandBase
    {
        private ElasticSecurityService _elasticSecurityService;
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            var msg = context.Update.Message;
            var chatId = msg.Chat.Id;
            var fromId = msg.From.Id;
            var partedText = msg.Text.Split(" ");
            var param1 = partedText.ValueOfIndex(1);

            _telegramService = new TelegramService(context);
            _elasticSecurityService = new ElasticSecurityService(msg);

            if (fromId.IsSudoer())
            {
                switch (param1)
                {
                    case "sync":
                        await _telegramService.SendTextAsync("Memperbarui cache..");
                        // await _elasticSecurityService.UpdateCacheAsync();
                        await Sync.SyncGBanToLocalAsync();

                        await _telegramService.EditAsync("Selesai memperbarui..");

                        break;

                    default:
                        if (msg.ReplyToMessage != null)
                        {
                            var repMsg = msg.ReplyToMessage;
                            var userId = repMsg.From.Id;

                            Log.Information("Execute Global Ban");
                            await _telegramService.SendTextAsync("Mempersiapkan..");
                            await _telegramService.DeleteAsync(msg.MessageId);

                            var isBan = await _elasticSecurityService.IsExist(userId);
                            Log.Information($"IsBan: {isBan}");
                            if (isBan)
                            {
                                await _telegramService.EditAsync("Pengguna sudah di ban");
                            }
                            else
                            {
                                var data = new Dictionary<string, object>()
                                {
                                    {"user_id", userId},
                                    {"banned_by", fromId},
                                    {"banned_from", chatId}
                                };

                                await _telegramService.EditAsync("Menyimpan informasi..");
                                var save = await _elasticSecurityService.SaveBanAsync(data);
                                Log.Information($"SaveBan: {save}");

                                await _telegramService.EditAsync("Menulis ke Cache..");
                                // await _elasticSecurityService.UpdateCacheAsync();
                                await Sync.SyncGBanToLocalAsync();
                                

                                await _telegramService.EditAsync("Misi berhasil.");
                            }
                        }
                        else
                        {
                            await _telegramService.SendTextAsync("Balas seseorang yang mau di ban");
                        }

                        break;
                }
            }
            else
            {
                await _telegramService.SendTextAsync("Unauthorized");
            }

            await _telegramService.DeleteAsync(delay: 3000);
        }
    }
}