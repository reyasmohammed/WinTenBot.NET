using WinTenBot.Text;

namespace WinTenBot.Telegram
{
    public static class Chats
    {
        public static long ReduceChatId(this long chatId)
        {
            var chatIdStr = chatId.ToString();
            if (chatIdStr.StartsWith("-100"))
            {
                chatIdStr = chatIdStr.Substring(4);
            }

            return chatIdStr.ToInt64();
        }
    }
}