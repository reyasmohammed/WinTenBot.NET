using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Helpers
{
    public static class MessageHelper
    {
        public static string GetFileId(this Message message)
        {
            var fileId = "";
            switch (message.Type)
            {
                case MessageType.Document:
                    fileId = message.Document.FileId;
                    break;

                case MessageType.Photo:
                    fileId = message.Photo[0].FileId;
                    break;

                case MessageType.Video:
                    fileId = message.Video.FileId;
                    break;
            }

            return fileId;
        }

        public static string GetReducedFileId(this Message message)
        {
            return GetFileId(message).Substring(0, 17);
        }

        public static string GetTextWithoutCmd(this string message, bool withoutCmd = true)
        {
            var partsMsg = message.Split(' ');
            var text = message;
            if (withoutCmd && message.StartsWith("/"))
            {
                text = message.TrimStart(partsMsg[0].ToCharArray());
            }

            return text.Trim();
        }

        public static string GetMessageLink(this Message message)
        {
            var chatUsername = message.Chat.Username;
            var messageId = message.MessageId;

            var messageLink = $"https://t.me/{chatUsername}/{messageId}";

            if (chatUsername == "")
            {
                var trimmedChatId = message.Chat.Id.ToString().Substring(4);
                messageLink = $"https://t.me/c/{trimmedChatId}/{messageId}";
            }

            Log.Information($"MessageLink: {messageLink}");
            return messageLink;
        }

        public static async Task<bool> IsMustDelete(string words)
        {
            var isMust = false;
            var query = await new Query("word_filter")
                .ExecForSqLite(true)
                .GetAsync();

            var mapped = query.ToJson().MapObject<List<WordFilter>>();

            var partedWord = words.Split(" ");
            foreach (var word in partedWord)
            {
                foreach (WordFilter wordFilter in mapped)
                {
                    var forFilter = wordFilter.Word;
                    var isGlobal = wordFilter.IsGlobal;

                    if (forFilter == word.ToLower())
                    {
                        isMust = true;
                    }

                    // Log.Debug($"Is ({isGlobal}) '{word}' == '{forFilter}' ? {isMust}");

                    if (isMust) break;
                }
            }

            return isMust;
        }

        public static async Task CheckMessage(this RequestProvider requestProvider, Message message)
        {
            try
            {
                Log.Information("Starting check Message");

                var text = message.Text;
                if (!text.IsNullOrEmpty())
                {
                    var isMustDelete = await IsMustDelete(text);
                    Log.Information($"Message {message.MessageId} IsMustDelete: {isMustDelete}");

                    if (isMustDelete) await requestProvider.DeleteAsync(message.MessageId);
                }
                else
                {
                    Log.Information("No message Text for scan.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking message");
            }
        }

        public static async Task FindNotesAsync(this RequestProvider requestProvider, Message msg)
        {
            try
            {
                Log.Information("Starting find Notes in Cloud");
                InlineKeyboardMarkup inlineKeyboardMarkup = null;
                var notesService = new NotesService();

                var selectedNotes = await notesService.GetNotesBySlug(msg.Chat.Id, msg.Text);
                if (selectedNotes.Count > 0)
                {
                    var content = selectedNotes[0].Content;
                    var btnData = selectedNotes[0].BtnData;
                    if (btnData != "")
                    {
                        inlineKeyboardMarkup = btnData.ToReplyMarkup(2);
                    }

                    await requestProvider.SendTextAsync(content, inlineKeyboardMarkup);
                    inlineKeyboardMarkup = null;

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
            catch (Exception ex)
            {
                Log.Error(ex, "Error when getting Notes");
            }
        }

        public static async Task CheckHastagMessageAsync(this RequestProvider requestProvider)
        {
            var tagsService = new TagsService();
            Message msg = requestProvider.Message;

            if(!msg.Text.Contains("#")){
                Log.Information($"Message {msg.MessageId} is not contains any Hashtag.");
                return;
            }

            Log.Information("Tags Received..");
            var partsText = msg.Text.Split(new char[] { ' ', '\n', ',' })
                .Where(x => x.Contains("#"));

            var limitedTags = partsText.Take(5);

            //            int count = 1;
            foreach (var split in limitedTags)
            {
                Log.Information("Processing : " + split.TrimStart('#'));

                var tagData = await tagsService.GetTagByTag(msg.Chat.Id, split.TrimStart('#'));
                var json = tagData.ToJson();
                Log.Information(json);

                var content = tagData[0].Content;
                var buttonStr = tagData[0].BtnData;

                InlineKeyboardMarkup buttonMarkup = null;
                if (buttonStr != "")
                {
                    buttonMarkup = buttonStr.ToReplyMarkup(2);
                }

                await requestProvider.SendTextAsync(content, buttonMarkup);
            }

            if (partsText.Count() > limitedTags.Count())
            {
                await requestProvider.SendTextAsync("Due performance reason, we limit 5 batch call tags");
            }
        }
    }
}