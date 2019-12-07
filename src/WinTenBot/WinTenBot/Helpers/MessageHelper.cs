using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
    }
}