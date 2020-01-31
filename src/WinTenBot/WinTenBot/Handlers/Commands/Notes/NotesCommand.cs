using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers.Processors;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Notes
{
    public class NotesCommand : CommandBase
    {
        private RequestProvider _requestProvider;
        private NotesService _notesService;

        public NotesCommand()
        {
            _notesService = new NotesService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);

            var notesData = await _notesService.GetNotesByChatId(_requestProvider.Message.Chat.Id);

            var sendText = "Filters di Obrolan ini.";

            if (notesData.Rows.Count > 0)
            {
                foreach (DataRow note in notesData.Rows)
                {
                    sendText += $"\nID: {note["id_note"]} - ";
                    sendText += $"{note["slug"]}";
                }
            }
            else
            {
                sendText = "Tidak ada Filters di Grup ini." +
                           "\nUntuk menambahkannya ketik /addfilter";
            }

            await _notesService.UpdateCache(_requestProvider.Message.Chat.Id);

            await _requestProvider.SendTextAsync(sendText);
        }
    }
}