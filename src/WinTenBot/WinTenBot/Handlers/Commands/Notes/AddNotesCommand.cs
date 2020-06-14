﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Common;
using WinTenBot.Services;
using WinTenBot.Telegram;

namespace WinTenBot.Handlers.Commands.Notes
{
    public class AddNotesCommand : CommandBase
    {
        private NotesService _notesService;
        private TelegramService _telegramService;

        public AddNotesCommand()
        {
            _notesService = new NotesService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            var msg = context.Update.Message;

            await _telegramService.SendTextAsync("This feature currently disabled");
            return;
            
            if (msg.ReplyToMessage != null)
            {
                var repMsg = msg.ReplyToMessage;
                await _telegramService.SendTextAsync("Mengumpulkan informasi");

                var partsContent = repMsg.Text.Split(new[] {"\n\n"}, StringSplitOptions.None);
                var partsMsgText = msg.Text.GetTextWithoutCmd().Split("\n\n");

                Log.Information(msg.Text);
                Log.Information(repMsg.Text);
                Log.Information(partsContent.ToJson());
                Log.Information(partsMsgText.ToJson());

                var data = new Dictionary<string, object>()
                {
                    {"slug", partsMsgText[0]},
                    {"content", partsContent[0]},
                    {"chat_id", msg.Chat.Id},
                    {"user_id", msg.From.Id}
                };

                if (!partsMsgText.ValueOfIndex(1).IsNullOrEmpty())
                {
                    data.Add("btn_data", partsMsgText[1]);
                }

                await _telegramService.EditAsync("Menyimpan..");
                await _notesService.SaveNote(data);

                await _telegramService.EditAsync("Berhasil");
            }
        }
    }
}