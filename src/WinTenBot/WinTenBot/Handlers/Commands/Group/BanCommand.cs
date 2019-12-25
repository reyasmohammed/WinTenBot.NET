using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers.Commands.Group
{
    public class BanCommand:CommandBase
    {
            private ChatProcessor _chatProcessor;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);

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

            await _chatProcessor.DeleteAsync(msg.MessageId);

            var isAdmin = await _chatProcessor.IsAdminGroup();

            if (kickTargets[0].Id != msg.From.Id && isAdmin)
            {
                var isKicked = false;
                foreach (var target in kickTargets)
                {
                    var idTarget = target.Id;
                    var sendText = string.Empty;
                    
                    isKicked = await _chatProcessor.KickMemberAsync(target);

                    if (isKicked)
                    {
                        sendText = $"{target} berhasil di blokir ";

                        sendText += idTarget == fromId ? $"oleh Self-ban" : $".";
                    }
                    else
                    {
                        sendText = $"Gagal memblokir {idTarget}";
                    }

                    await _chatProcessor.AppendTextAsync(sendText);
                }

                if (isKicked)
                {
                    await _chatProcessor.AppendTextAsync($"Sebanyak {kickTargets.Count} berhasil di blokir.");
                }
                else
                {
                    await _chatProcessor.AppendTextAsync("Gagal memblokir bbrp anggota");
                }
            }
            else if (kickTargets[0].Id == fromId)
            {
                var idTarget = kickTargets[0];
                var isKicked = false;
                
                isKicked = await _chatProcessor.KickMemberAsync(idTarget);
                if (isKicked)
                {
                    var sendText = $"{idTarget} berhasil di blokir ";
                    sendText += idTarget.Id == fromId ? $"oleh Self-ban" : $".";
                    await _chatProcessor.AppendTextAsync(sendText);
                }
                else
                {
                    var sendTexts = $"Blokir {idTarget} gagal.";
                    await _chatProcessor.SendAsync(sendTexts);
                }
            }
            else
            {
                await _chatProcessor.SendAsync("Hanya admin yang bisa mengeksekusi");
            }
        }
    }
}