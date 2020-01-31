using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Helpers.Processors;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Group
{
    public class BanCommand:CommandBase
    {
            private RequestProvider _requestProvider;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);

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

            await _requestProvider.DeleteAsync(msg.MessageId);

            var isAdmin = await _requestProvider.IsAdminGroup();

            if (kickTargets[0].Id != msg.From.Id && isAdmin)
            {
                var isKicked = false;
                foreach (var target in kickTargets)
                {
                    var idTarget = target.Id;
                    var sendText = string.Empty;
                    
                    isKicked = await _requestProvider.KickMemberAsync(target);

                    if (isKicked)
                    {
                        sendText = $"{target} berhasil di blokir ";

                        sendText += idTarget == fromId ? $"oleh Self-ban" : $".";
                    }
                    else
                    {
                        sendText = $"Gagal memblokir {idTarget}";
                    }

                    await _requestProvider.AppendTextAsync(sendText);
                }

                if (isKicked)
                {
                    await _requestProvider.AppendTextAsync($"Sebanyak {kickTargets.Count} berhasil di blokir.");
                }
                else
                {
                    await _requestProvider.AppendTextAsync("Gagal memblokir bbrp anggota");
                }
            }
            else if (kickTargets[0].Id == fromId)
            {
                var idTarget = kickTargets[0];
                var isKicked = false;
                
                isKicked = await _requestProvider.KickMemberAsync(idTarget);
                if (isKicked)
                {
                    var sendText = $"{idTarget} berhasil di blokir ";
                    sendText += idTarget.Id == fromId ? $"oleh Self-ban" : $".";
                    await _requestProvider.AppendTextAsync(sendText);
                }
                else
                {
                    var sendTexts = $"Blokir {idTarget} gagal.";
                    await _requestProvider.SendTextAsync(sendTexts);
                }
            }
            else
            {
                await _requestProvider.SendTextAsync("Hanya admin yang bisa mengeksekusi");
            }
        }
    }
}