using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Helpers.Processors;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Group
{
    public class KickCommand : CommandBase
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

                    // foreach (var target in userTargets)
                    // {
                    //     idTargets.Add(target.Id);
                    // }
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

                    // await _chatProcessor.AppendTextAsync($"Sedang menendang {idTarget}");
                    isKicked = await _requestProvider.KickMemberAsync(target);
                    await _requestProvider.UnbanMemberAsync(target);

                    if (isKicked)
                    {
                        sendText = $"{target} berhasil di tendank ";

                        sendText += idTarget == fromId ? $"oleh Self-kick" : $".";
                    }
                    else
                    {
                        sendText = $"Gagal menendang {idTarget}";
                    }

                    await _requestProvider.AppendTextAsync(sendText);
                }

                if (isKicked)
                {
                    await _requestProvider.AppendTextAsync($"Sebanyak {kickTargets.Count} berhasil di tendang.");
                }
                else
                {
                    await _requestProvider.AppendTextAsync("Gagal menendang bbrp anggota");
                }
            }
            else if (kickTargets[0].Id == fromId)
            {
                var idTarget = kickTargets[0];
                var isKicked = false;
                // await _chatProcessor.AppendTextAsync($"Sedang menendang {idTarget}");
                isKicked = await _requestProvider.KickMemberAsync(idTarget);
                if (isKicked)
                {
                    var sendText = $"{idTarget} berhasil di tendang ";
                    sendText += idTarget.Id == fromId ? $"oleh Self-kick" : $".";
                    await _requestProvider.AppendTextAsync(sendText);
                }
                else
                {
                    var sendTexts = $"Tendang {idTarget} gagal.";
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