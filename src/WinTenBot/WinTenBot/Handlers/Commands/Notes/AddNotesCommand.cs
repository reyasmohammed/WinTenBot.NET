using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Notes
{
    public class AddNotesCommand : CommandBase
    {
        private NotesService _notesService;
        private TelegramProvider _telegramProvider;

        public AddNotesCommand()
        {
            _notesService = new NotesService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            var msg = context.Update.Message;

            if (msg.ReplyToMessage != null)
            {
                var repMsg = msg.ReplyToMessage;
                await _telegramProvider.SendTextAsync("Mengumpulkan informasi");

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

                if (partsMsgText[1] != "")
                {
                    data.Add("btn_data", partsMsgText[1]);
                }

                await _telegramProvider.EditAsync("Menyimpan..");
                await _notesService.SaveNote(data);

                await _telegramProvider.EditAsync("Berhasil");
            }
        }
    }
}