﻿using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    public class GlobalReportCommand:CommandBase
    {
        private TelegramProvider _telegramProvider;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            var msg = _telegramProvider.Message;

            if (msg.ReplyToMessage != null)
            {
                var chatTitle = msg.Chat.Title;
                var chatId = msg.Chat.Id;
                var from = msg.From;
                var reason = msg.Text.GetTextWithoutCmd();

                var repMsg = msg.ReplyToMessage;
                var repFrom = repMsg.From;
                
                var msgBuild = new StringBuilder();

                msgBuild.AppendLine("‼️ <b>Global Report</b>");
                msgBuild.AppendLine($"<b>ChatTitle:</b> {chatTitle}");
                msgBuild.AppendLine($"<b>ChatID:</b> {chatId}");
                msgBuild.AppendLine($"<b>Reporter:</b> {from}");
                msgBuild.AppendLine($"<b>Reported:</b> {repFrom}");
                msgBuild.AppendLine($"<b>Reason:</b> {reason}");
                msgBuild.AppendLine($"\nTerima kasih sudah melaporkan!");

                var mentionAdmin = await _telegramProvider.GetMentionAdminsStr();

                var isAdmin = await _telegramProvider.IsAdminGroup();
                if (!isAdmin) msgBuild.AppendLine(mentionAdmin);
                
                var sendText = msgBuild.ToString().Trim();
                await _telegramProvider.ForwardMessageAsync(repMsg.MessageId);
                await _telegramProvider.SendTextAsync(sendText);
            }
            else
            {
                var sendText = "ℹ️ <b>Balas</b> pesan yang mau di laporkan" +
                               "\n\n<b>Catatan:</b> GReport (Global Report) akan melaporkan pengguna ke Tim @WinTenDev " +
                               "dan memanggil admin di Grup ini.";
                await _telegramProvider.SendTextAsync(sendText);
            }
        }
    }
}