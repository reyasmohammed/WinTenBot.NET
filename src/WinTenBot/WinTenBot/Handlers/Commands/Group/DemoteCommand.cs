﻿using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Providers;
using WinTenBot.Services;
using WinTenBot.Telegram;

namespace WinTenBot.Handlers.Commands.Group
{
    public class DemoteCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            var msg = context.Update.Message;
            if (msg.ReplyToMessage != null)
            {
                msg = msg.ReplyToMessage;
            }

            var userId = msg.From.Id;
            var nameLink = msg.GetFromNameLink();

            var sendText = $"{nameLink} diturunkan dari admin";

            var promote = await _telegramService.DemoteChatMemberAsync(userId);
            if (!promote.IsSuccess)
            {
                var errorCode = promote.ErrorCode;
                var errorMessage = promote.ErrorMessage;

                sendText = $"Demote {nameLink} gagal" +
                           $"\nPesan: {errorMessage}";
            }

            await _telegramService.SendTextAsync(sendText);
        }
    }
}