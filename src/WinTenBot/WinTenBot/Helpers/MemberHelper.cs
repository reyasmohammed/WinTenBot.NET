using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using WinTenBot.Providers;
using WinTenBot.Services;

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

        public static async Task CheckUsername(this RequestProvider requestProvider, Message message)
        {
            var fromUser = message.From;
            var noUsername = fromUser.IsNoUsername();
            ConsoleHelper.WriteLine($"{fromUser} IsNoUsername: {noUsername}");

            if (noUsername)
            {
                await requestProvider.SendTextAsync($"{fromUser} belum memasang username");
            }
        }

        public static async Task AfkCheck(this RequestProvider requestProvider, Message message)
        {
            var afkService = new AfkService();

            if (message.ReplyToMessage != null)
            {
                var repMsg = message.ReplyToMessage;
                var isAfkReply = await afkService.IsAfkAsync(repMsg);
                if (isAfkReply)
                    await requestProvider.SendTextAsync($"{repMsg.GetFromNameLink()} sedang afk");
            }

            var isAfk = await afkService.IsAfkAsync(message);
            if (isAfk)
            {
                await requestProvider.SendTextAsync($"{message.GetFromNameLink()} sudah tidak afk");

                var data = new Dictionary<string, object>()
                {
                    {"chat_id", message.Chat.Id},
                    {"user_id", message.From.Id},
                    {"is_afk", 0},
                    {"afk_reason", ""}
                };

                await afkService.SaveAsync(data);
                await afkService.UpdateCacheAsync();
            }
        }
        
        public static async Task CheckGlobalBanAsync(this RequestProvider requestProvider, Message message)
        {
            var userId = message.From.Id;
            var user = message.From;
            var messageId = message.MessageId;

            // var isBan = await _elasticSecurityService.IsExist(userId);
            var isBan = await user.IsBanInCache();
            ConsoleHelper.WriteLine($"IsBan: {isBan}");
            if (isBan)
            {
                await requestProvider.DeleteAsync(messageId);
                await requestProvider.KickMemberAsync(user);
                await requestProvider.UnbanMemberAsync(user);
            }
        }
    }
}