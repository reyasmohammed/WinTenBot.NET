using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WinTenBot.Helpers
{
    public static class MemberHelper
    {
        public static string GetNameLink(int userId, string Name)
        {
            return $"<a href='tg://user?id={userId}'>{Name}</a>";
        }
        
        public static string GetFromNameLink(this Message message)
        {
            var firstName = message.From.FirstName;
            var lastName = message.From.LastName;
            
            return $"<a href='tg://user?id={message.From.Id}'>{(firstName + " " + lastName).Trim()}</a>";
        }

        public static async Task<bool> IsAdminGroup(ITelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var userId = message.From.Id;
            var isAdmin = false;

            var admins = await client.GetChatAdministratorsAsync(chatId);
            foreach (var admin in admins)
            {
                if (userId == admin.User.Id)
                {
                    isAdmin = true;
                }
            }

            return isAdmin;
        }
    }
}