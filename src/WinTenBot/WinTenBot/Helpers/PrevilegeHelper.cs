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
            var sudoers = BotSettings.GlobalConfiguration.GetSection("Sudoers").Get<List<string>>();
            var match = sudoers.FirstOrDefault(x => x == userId.ToString());
            
            if (match != null)
            {
                isSudoer = true;
            }
            Log.Information($"UserId: {userId} IsSudoer: {isSudoer}");
            return  isSudoer;
        }
        
        public static bool IsSudoer(this TelegramProvider telegramProvider)
        {
            bool isSudoer = false;
            var userId = telegramProvider.Message.From.Id;
            var sudoers = BotSettings.GlobalConfiguration.GetSection("Sudoers").Get<List<string>>();
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
        
        public static async Task<bool> IsAdminGroup(this TelegramProvider telegramProvider, int userId = -1)
        {
            var message = telegramProvider.MessageOrEdited;
            var client = telegramProvider.Client;
            
            var chatId = message.Chat.Id;
            var fromId = message.From.Id;
            var isAdmin = false;
            
            if (IsPrivateChat(telegramProvider)) return false;
            if (userId >= 0) fromId = userId;
            
            var admins = await client.GetChatAdministratorsAsync(chatId);
            foreach (var admin in admins)
            {
                if (fromId == admin.User.Id)
                {
                    isAdmin = true;
                }
            }
            
            Log.Information($"UserId {fromId} IsAdmin: {isAdmin}");

            return isAdmin;
        }

        public static async Task<ChatMember[]> GetAllAdmins(this TelegramProvider telegramProvider)
        {
            var client = telegramProvider.Client;
            var message = telegramProvider.Message;
            var chatId = message.Chat.Id;
            
            var allAdmins = await client.GetChatAdministratorsAsync(chatId);
            if(BotSettings.IsDevelopment)
                Log.Information($"All Admin on {chatId} {allAdmins.ToJson(true)}");

            return allAdmins;
        }
    }
}