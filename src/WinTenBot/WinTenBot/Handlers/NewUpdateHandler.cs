using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers
{
    public class NewUpdateHandler : IUpdateHandler
    {
        private readonly AfkService _afkService;
        private NotesService _notesService;
        private RequestProvider _requestProvider;

        public NewUpdateHandler()
        {
            _afkService = new AfkService();
            _notesService = new NotesService();
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var message = context.Update.Message ?? context.Update.CallbackQuery.Message;
            
            Log.Information("New Update");

            Parallel.Invoke(
                async () => await AfkCheck(message), 
                async () => await CheckUsername(message), 
                async () => await FindNotesAsync(message), 
                async () => await HitActivity(message));

            if (context.Update.CallbackQuery == null)
            {
                Parallel.Invoke(async () => await CheckMessage(message));
            }

            if (!_requestProvider.IsPrivateChat())
            {
                Parallel.Invoke(async () => await CheckGlobalBanAsync(message));
            }

            await next(context, cancellationToken);
        }

        private async Task AfkCheck(Message message)
        {
            if (message.ReplyToMessage != null)
            {
                var repMsg = message.ReplyToMessage;
                var isAfkReply = await _afkService.IsAfkAsync(repMsg);
                if (isAfkReply)
                    await $"{repMsg.GetFromNameLink()} sedang afk".SendTextAsync();
            }

            var isAfk = await _afkService.IsAfkAsync(message);
            if (isAfk)
            {
                await $"{message.GetFromNameLink()} sudah tidak afk".SendTextAsync();

                var data = new Dictionary<string, object>()
                {
                    {"chat_id", message.Chat.Id},
                    {"user_id", message.From.Id},
                    {"is_afk", 0},
                    {"afk_reason", ""}
                };

                await _afkService.SaveAsync(data);
                await _afkService.UpdateCacheAsync();
            }
        }

        private async Task CheckGlobalBanAsync(Message message)
        {
            var userId = message.From.Id;
            var user = message.From;
            var messageId = message.MessageId;

            // var isBan = await _elasticSecurityService.IsExist(userId);
            var isBan = await user.IsBanInCache();
            ConsoleHelper.WriteLine($"IsBan: {isBan}");
            if (isBan)
            {
                await _requestProvider.DeleteAsync(messageId);
                await _requestProvider.KickMemberAsync(user);
                await _requestProvider.UnbanMemberAsync(user);
            }
        }

        private async Task CheckUsername(Message message)
        {
            var fromUser = message.From;
            var noUsername = fromUser.IsNoUsername();
            ConsoleHelper.WriteLine($"{fromUser} IsNoUsername: {noUsername}");

            if (noUsername)
            {
                await $"{fromUser} belum memasang username".SendTextAsync();
            }
        }

        private async Task HitActivity(Message message)
        {
            var data = new Dictionary<string, object>()
            {
                {"via_bot", "ZiziBeta"},
                {"message_type", message.Type.ToString()},
                {"from_id", message.From.Id},
                {"from_first_name", message.From.FirstName},
                {"from_last_name", message.From.LastName},
                {"from_username", message.From.Username},
                {"from_lang_code", message.From.LanguageCode},
                {"chat_id", message.Chat.Id},
                {"chat_username", message.Chat.Username},
                {"chat_type", message.Chat.Type.ToString()},
                {"chat_title", message.Chat.Title},
            };

            var insertHit = await new Query("hit_activity")
                .ExecForMysql()
                .InsertAsync(data);
            ConsoleHelper.WriteLine($"Insert Hit: {insertHit}");
        }

        private async Task FindNotesAsync(Message msg)
        {
            InlineKeyboardMarkup _replyMarkup = null;

            var selectedNotes = await _notesService.GetNotesBySlug(msg.Chat.Id, msg.Text);
            if (selectedNotes.Count > 0)
            {
                var content = selectedNotes[0].Content;
                var btnData = selectedNotes[0].BtnData;
                if (btnData != "")
                {
                    _replyMarkup = btnData.ToReplyMarkup(2);
                }

                await _requestProvider.SendTextAsync(content, _replyMarkup);
                _replyMarkup = null;

                foreach (var note in selectedNotes)
                {
                    Log.Debug(note.ToJson());
                }
            }
            else
            {
                Log.Debug("No rows result set in Notes");
            }
        }

        private async Task CheckMessage(Message message)
        {
            var text = message.Text;
            var isMustDelete = await MessageHelper.IsMustDelete(text);
            Log.Debug($"Message {message.MessageId} IsMustDelete: {isMustDelete}");

            if (isMustDelete) await _requestProvider.DeleteAsync(message.MessageId);
        }
    }
}