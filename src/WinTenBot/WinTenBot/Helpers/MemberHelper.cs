using System.Data;
using System.Linq;
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
        
        public static async Task<bool> IsBanInCache(this User user)
        {
            var filtered = new DataTable(null);
            var data = await "fban_user.json".ReadCacheAsync();
            var userId = user.Id;
            
            ConsoleHelper.WriteLine($"Checking {user} in Global Ban Cache");
            var search = data.AsEnumerable()
                .Where(row => row.Field<string>("user_id") == userId.ToString());
            if (search.Any())
            {
                filtered = search.CopyToDataTable();
            }

            ConsoleHelper.WriteLine($"Caches found: {filtered.ToJson()}");
            return filtered.Rows.Count > 0;
        }

        public static bool IsNoUsername(this User user)
        {
            return user.Username == null;
        }
    }
}