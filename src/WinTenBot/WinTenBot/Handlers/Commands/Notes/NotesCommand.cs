using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers.Processors;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Notes
{
    public class NotesCommand : CommandBase
    {
        private ChatProcessor _chatProcessor;
        private NotesService _notesService;

        public NotesCommand()
        {
            _notesService = new NotesService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);

            var notesData = await _notesService.GetNotesByChatId(_chatProcessor.ChatId);

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

            await _notesService.UpdateCache(_chatProcessor.ChatId);

            await _chatProcessor.SendAsync(sendText);
        }
    }
}