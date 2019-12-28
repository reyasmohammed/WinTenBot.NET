using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Security
{
    public class DelBanCommand : CommandBase
    {
        private ChatProcessor _chatProcessor;
        private ElasticSecurityService _elasticSecurityService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            var msg = context.Update.Message;
            var chatId = msg.Chat.Id;
            var fromId = msg.From.Id;

            _chatProcessor = new ChatProcessor(context);
            _elasticSecurityService = new ElasticSecurityService(msg);

            if (fromId.IsSudoer())
            {
                if (msg.ReplyToMessage != null)
                {
                    var repMsg = msg.ReplyToMessage;
                    var userId = repMsg.From.Id;

                    ConsoleHelper.WriteLine("Execute Global DelBan");
                    await _chatProcessor.SendAsync("Mempersiapkan..");
                    await _chatProcessor.DeleteAsync(msg.MessageId);

                    var isBan = await _elasticSecurityService.IsExist(userId);
                    ConsoleHelper.WriteLine($"IsBan: {isBan}");
                    if (isBan)
                    {
                        await _chatProcessor.EditAsync("Memperbarui informasi..");
                        var save = await _elasticSecurityService.DeleteBanAsync(userId);
                        ConsoleHelper.WriteLine($"SaveBan: {save}");

                        await _chatProcessor.EditAsync("Menulis ke Cache..");
                        await _elasticSecurityService.UpdateCacheAsync();

                        await _chatProcessor.EditAsync("Misi berhasil.");
                    }
                    else
                    {
                        await _chatProcessor.EditAsync("Pengguna tidak di ban");
                    }
                }
                else
                {
                    await _chatProcessor.SendAsync("Balas seseorang yang mau di ban");
                }
            }
            else
            {
                await _chatProcessor.SendAsync("Unauthorized");
            }

            await _chatProcessor.DeleteAsync(delay: 3000);
        }
    }
}