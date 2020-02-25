using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Notes
{
    public class AddNotesCommand : CommandBase
    {
        private RequestProvider _requestProvider;
        private NotesService _notesService;

        public AddNotesCommand()
        {
            _notesService = new NotesService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;

            if (msg.ReplyToMessage != null)
            {
                var repMsg = msg.ReplyToMessage;
                await _requestProvider.SendTextAsync("Mengumpulkan informasi");

                var partsContent = repMsg.Text.Split(new[] {"\n\n"}, StringSplitOptions.None);
                var partsMsgText = msg.Text.GetTextWithoutCmd().Split("\n\n");

                ConsoleHelper.WriteLine(msg.Text);
                ConsoleHelper.WriteLine(repMsg.Text);
                ConsoleHelper.WriteLine(partsContent.ToJson());
                ConsoleHelper.WriteLine(partsMsgText.ToJson());

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

                await _requestProvider.EditAsync("Menyimpan..");
                await _notesService.SaveNote(data);

                await _requestProvider.EditAsync("Berhasil");
            }
        }
    }
}