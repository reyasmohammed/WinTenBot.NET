using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Services;
using WinTenBot.Telegram;

namespace WinTenBot.Handlers.Commands.Group
{
    public class BanCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);

            var kickTargets = new List<User>();

            var msg = context.Update.Message;
            Message repMsg = null;
            var fromId = msg.From.Id;
            // var idTargets = new List<int>();

            kickTargets.Add(msg.From);

            if (msg.ReplyToMessage != null)
            {
                repMsg = msg.ReplyToMessage;
                // idTarget = repMsg.From.id;
                kickTargets.Add(repMsg.From);

                if (repMsg.NewChatMembers != null)
                {
                    kickTargets.Clear();
                    var userTargets = repMsg.NewChatMembers;
                    kickTargets.AddRange(userTargets);
                }
            }

            await _telegramService.DeleteAsync(msg.MessageId);

            var isAdmin = await _telegramService.IsAdminGroup();

            if (kickTargets[0].Id != msg.From.Id && isAdmin)
            {
                var isKicked = false;
                foreach (var target in kickTargets)
                {
                    var idTarget = target.Id;
                    var sendText = string.Empty;

                    isKicked = await _telegramService.KickMemberAsync(target);

                    if (isKicked)
                    {
                        sendText = $"{target} berhasil di blokir ";

                        sendText += idTarget == fromId ? $"oleh Self-ban" : $".";
                    }
                    else
                    {
                        sendText = $"Gagal memblokir {idTarget}";
                    }

                    await _telegramService.AppendTextAsync(sendText);
                }

                if (isKicked)
                {
                    await _telegramService.AppendTextAsync($"Sebanyak {kickTargets.Count} berhasil di blokir.");
                }
                else
                {
                    await _telegramService.AppendTextAsync("Gagal memblokir bbrp anggota");
                }
            }
            else if (kickTargets[0].Id == fromId)
            {
                var idTarget = kickTargets[0];
                var isKicked = false;

                isKicked = await _telegramService.KickMemberAsync(idTarget);
                if (isKicked)
                {
                    var sendText = $"{idTarget} berhasil di blokir ";
                    sendText += idTarget.Id == fromId ? $"oleh Self-ban" : $".";
                    await _telegramService.AppendTextAsync(sendText);
                }
                else
                {
                    var sendTexts = $"Blokir {idTarget} gagal.";
                    await _telegramService.SendTextAsync(sendTexts);
                }
            }
            else
            {
                await _telegramService.SendTextAsync("Hanya admin yang bisa mengeksekusi");
            }
        }
    }
}