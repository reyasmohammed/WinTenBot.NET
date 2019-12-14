using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers
{
    public class GenericMessageHandler : IUpdateHandler
    {
        private CasBanProvider _casBanProvider;
        private ChatProcessor _chatProcessor;
        private NotesService _notesService;
        private IReplyMarkup _replyMarkup;

        public GenericMessageHandler()
        {
            _casBanProvider = new CasBanProvider();
            _notesService = new NotesService();
            _replyMarkup = null;
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            Message msg = context.Update.Message;

            ConsoleHelper.WriteLine(msg.ToJson());
            ConsoleHelper.WriteLine(msg.Text);

            if (Bot.HostingEnvironment.IsProduction())
            {
                await _casBanProvider.IsCasBan(msg.From.Id);
            }
            
            var selectedNotes = await _notesService.GetNotesBySlug(msg.Chat.Id, msg.Text);
            if (selectedNotes.Rows.Count > 0)
            {
                var content = selectedNotes.Rows[0]["content"].ToString();
                var btnData = selectedNotes.Rows[0]["btn_data"].ToString();
                if (btnData != "")
                {
                    _replyMarkup = btnData.ToReplyMarkup(2);
                }
                
                await _chatProcessor.SendAsync(content,_replyMarkup);
                _replyMarkup = null;

                foreach (var note in selectedNotes.Rows)
                {
                    ConsoleHelper.WriteLine(note.ToJson());
                }
            }
            else
            {
                ConsoleHelper.WriteLine("No rows result set in Notes");
            }

            //            await context.Bot.Client.SendTextMessageAsync(
            //                msg.Chat, "You said:\n" + msg.Text
            //            );

            await next(context);
        }
    }
}