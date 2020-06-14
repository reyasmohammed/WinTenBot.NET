﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WinTenBot.Common;
using WinTenBot.Model;
using WinTenBot.Services;

namespace WinTenBot.Telegram
{
    public static class Privilege
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
        
        public static bool IsSudoer(this TelegramService telegramService)
        {
            bool isSudoer = false;
            var userId = telegramService.Message.From.Id;
            var sudoers = BotSettings.GlobalConfiguration.GetSection("Sudoers").Get<List<string>>();
            var match = sudoers.FirstOrDefault(x => x == userId.ToString());
            
            if (match != null)
            {
                isSudoer = true;
            }
            Log.Information($"UserId: {userId} IsSudoer: {isSudoer}");
            return  isSudoer;
        }
        
        public static async Task<bool> IsAdminOrPrivateChat(this TelegramService telegramService)
        {
            var isAdmin = await IsAdminGroup(telegramService);
            var isPrivateChat = IsPrivateChat(telegramService);

            return isAdmin || isPrivateChat;
        }
        
        public static bool IsPrivateChat(this TelegramService telegramService)
        {
            var messageOrEdited = telegramService.MessageOrEdited;
            return messageOrEdited.Chat.Type == ChatType.Private;
        }
        
        public static async Task<bool> IsAdminGroup(this TelegramService telegramService, int userId = -1)
        {
            var message = telegramService.MessageOrEdited;
            var client = telegramService.Client;
            
            var chatId = message.Chat.Id;
            var fromId = message.From.Id;
            var isAdmin = false;
            
            if (IsPrivateChat(telegramService)) return false;
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

        public static async Task<ChatMember[]> GetAllAdmins(this TelegramService telegramService)
        {
            var client = telegramService.Client;
            var message = telegramService.Message;
            var chatId = message.Chat.Id;
            
            var allAdmins = await client.GetChatAdministratorsAsync(chatId);
            if(BotSettings.IsDevelopment)
                Log.Information($"All Admin on {chatId} {allAdmins.ToJson(true)}");

            return allAdmins;
        }
    }
}