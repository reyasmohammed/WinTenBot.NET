﻿using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Services;
using WinTenBot.Telegram;
using WinTenBot.Tools;

namespace WinTenBot.Handlers.Commands.Rss
{
    public class RssPullCommand : CommandBase
    {
        private TelegramService _telegramService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            // ChatHelper.Init(context);
            _telegramService = new TelegramService(context);

            // var chatId = ChatHelper.Message.Chat.Id.ToString();
            // var isAdmin = await ChatHelper.IsAdminGroup();

            var chatId = _telegramService.Message.Chat.Id;
            var isAdmin = await _telegramService.IsAdminGroup()
                .ConfigureAwait(false);

            if (isAdmin || _telegramService.IsPrivateChat())
            {
#pragma warning disable 4014
                Task.Run(async () =>
#pragma warning restore 4014
                {
                    // Thread.CurrentThread.IsBackground = true;

                    await _telegramService.SendTextAsync("Sedang memeriksa RSS feed baru..")
                        .ConfigureAwait(false);
                    // await "Sedang memeriksa RSS feed baru..".SendTextAsync();

                    var newRssCount = await RssBroadcaster.ExecBroadcasterAsync(chatId)
                        .ConfigureAwait(false);
                    if (newRssCount == 0)
                    {
                        await _telegramService.EditAsync("Tampaknya tidak ada RSS baru saat ini")
                            .ConfigureAwait(false);
                        // await "Tampaknya tidak ada RSS baru saat ini".EditAsync();
                    }

                    // ChatHelper.Close();
                }, cancellationToken);
            }
        }
    }
}