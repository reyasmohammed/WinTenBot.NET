using System.Collections.Generic;
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
        private AfkService _afkService;

        public GenericMessageHandler()
        {
            _casBanProvider = new CasBanProvider();
            _notesService = new NotesService();
            _afkService = new AfkService();
            _replyMarkup = null;
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            Message msg = context.Update.Message;

            ConsoleHelper.WriteLine(msg.ToJson());
            ConsoleHelper.WriteLine(msg.Text);


            if (msg.Text == "ping")
            {
                // run /ping?
                
            }
            
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

            await AfkCheck(msg);

            await next(context);
        }

        private async Task AfkCheck(Message message)
        {
            if (message.ReplyToMessage != null)
            {
                var repMsg = message.ReplyToMessage;
                var isAfkReply = await _afkService.IsAfkAsync(repMsg);
                if (isAfkReply)
                    await _chatProcessor.SendAsync($"{repMsg.GetFromNameLink()} sedang afk");
            }
            
            var isAfk = await _afkService.IsAfkAsync(message);
            if (isAfk)
            {
                await _chatProcessor.SendAsync($"{message.GetFromNameLink()} sudah tidak afk");
                
                var data = new Dictionary<string, object>()
                {
                    {"chat_id",message.Chat.Id},
                    {"user_id", message.From.Id},
                    {"is_afk", 0},
                    {"afk_reason",""}
                };

                await _afkService.SaveAsync(data);
            }

            await _afkService.UpdateCacheAsync();

        }
    }
}