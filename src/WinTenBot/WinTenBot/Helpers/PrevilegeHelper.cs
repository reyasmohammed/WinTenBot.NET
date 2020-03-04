using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WinTenBot.Model;
using WinTenBot.Providers;

namespace WinTenBot.Helpers
{
    public static class PrevilegeHelper
    {
        public static bool IsSudoer(this int userId)
        {
            bool isSudoer = false;
            var sudoers = Bot.GlobalConfiguration.GetSection("Sudoers").Get<List<string>>();
            var match = sudoers.FirstOrDefault(x => x == userId.ToString());
            
            if (match != null)
            {
                isSudoer = true;
            }
            Log.Information($"UserId: {userId} IsSudoer: {isSudoer}");
            return  isSudoer;
        }
        
        public static async Task<bool> IsAdminOrPrivateChat(this TelegramProvider telegramProvider)
        {
            var isAdmin = await IsAdminGroup(telegramProvider);
            var isPrivateChat = IsPrivateChat(telegramProvider);

            return isAdmin || isPrivateChat;
        }
        
        public static bool IsPrivateChat(this TelegramProvider telegramProvider)
        {
            var messageOrEdited = telegramProvider.MessageOrEdited;
            return messageOrEdited.Chat.Type == ChatType.Private;
        }
        
        public static async Task<bool> IsAdminGroup(this TelegramProvider telegramProvider)
        {
            var message = telegramProvider.MessageOrEdited;
            var client = telegramProvider.Client;
            
            var chatId = message.Chat.Id;
            var userId = message.From.Id;
            var isAdmin = false;

            if (IsPrivateChat(telegramProvider)) return false;
            
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