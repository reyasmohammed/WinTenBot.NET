using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WinTenBot.Model;
using WinTenBot.Providers;

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
                text =  message.TrimStart(partsMsg[0].ToCharArray());
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
            
            ConsoleHelper.WriteLine($"MessageLink: {messageLink}");
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
                    
                    Log.Debug($"Is ({isGlobal}) '{word}' == '{forFilter}' ? {isMust}");
                    
                    if(isMust) break;
                }
            }

            return isMust;
        }
    }
}