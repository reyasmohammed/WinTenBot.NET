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
    public class GlobalBanCommand : CommandBase
    {
        private ElasticSecurityService _elasticSecurityService;
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            var msg = context.Update.Message;
            var chatId = msg.Chat.Id;
            var fromId = msg.From.Id;
            var partedText = msg.Text.Split(" ");
            var param1 = partedText.ValueOfIndex(1);

            _telegramProvider = new TelegramProvider(context);
            _elasticSecurityService = new ElasticSecurityService(msg);

            if (fromId.IsSudoer())
            {
                switch (param1)
                {
                    case "sync":
                        await _telegramProvider.SendTextAsync("Memperbarui cache..");
                        await _elasticSecurityService.UpdateCacheAsync();
                        await _telegramProvider.EditAsync("Selesai memperbarui..");

                        break;

                    default:
                        if (msg.ReplyToMessage != null)
                        {
                            var repMsg = msg.ReplyToMessage;
                            var userId = repMsg.From.Id;

                            Log.Information("Execute Global Ban");
                            await _telegramProvider.SendTextAsync("Mempersiapkan..");
                            await _telegramProvider.DeleteAsync(msg.MessageId);

                            var isBan = await _elasticSecurityService.IsExist(userId);
                            Log.Information($"IsBan: {isBan}");
                            if (isBan)
                            {
                                await _telegramProvider.EditAsync("Pengguna sudah di ban");
                            }
                            else
                            {
                                var data = new Dictionary<string, object>()
                                {
                                    {"user_id", userId},
                                    {"banned_by", fromId},
                                    {"banned_from", chatId}
                                };

                                await _telegramProvider.EditAsync("Menyimpan informasi..");
                                var save = await _elasticSecurityService.SaveBanAsync(data);
                                Log.Information($"SaveBan: {save}");

                                await _telegramProvider.EditAsync("Menulis ke Cache..");
                                await _elasticSecurityService.UpdateCacheAsync();

                                await _telegramProvider.EditAsync("Misi berhasil.");
                            }
                        }
                        else
                        {
                            await _telegramProvider.SendTextAsync("Balas seseorang yang mau di ban");
                        }

                        break;
                }
            }
            else
            {
                await _telegramProvider.SendTextAsync("Unauthorized");
            }

            await _telegramProvider.DeleteAsync(delay: 3000);
        }
    }
}