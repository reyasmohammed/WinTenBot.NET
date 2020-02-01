using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using WinTenBot.Model;
using WinTenBot.Providers;
using File = System.IO.File;

namespace WinTenBot.Helpers
{
    public static class BotHelper
    {
        public static async Task<string> GetUrlStart(this RequestProvider requestProvider, string param)
        {
            var bot = await requestProvider.Client.GetMeAsync();
            var username = bot.Username;
            return $"https://t.me/{username}?{param}";
        }
        
        public static async Task<User> GetBotUser(this RequestProvider requestProvider)
        {
            return await requestProvider.Client.GetMeAsync();
        }

        public static async Task<bool> IsBotAdded(this User[] users)
        {
            var me = await Bot.Client.GetMeAsync();
            return (from user in users where user.Id == me.Id select user.Id == me.Id).FirstOrDefault();
        }

        public static bool IsRestricted()
        {
            return Bot.GlobalConfiguration["CommonConfig:IsRestricted"].ToBool();
        }
        public static bool CheckRestriction(this long chatId)
        {
            var isRestricted = false;
            var globalRestrict = IsRestricted();
            var sudoers = Bot.GlobalConfiguration.GetSection("RestrictArea").Get<List<string>>();
            var match = sudoers.FirstOrDefault(x => x == chatId.ToString());

            Log.Information($@"Global Restriction: {globalRestrict}");
            if (match == null && globalRestrict)
            {
                isRestricted =  true;
            }
            Log.Information($"ChatId: {chatId} IsRestricted: {isRestricted}");
            return isRestricted;
        }

        public static async Task<bool> EnsureChatRestriction(this RequestProvider requestProvider)
        {
            var chatId = requestProvider.Message.Chat.Id;

            if (!chatId.CheckRestriction()) return false;
            
            Log.Information("I must leave right now!");
            var msgOut = $"Sepertinya saya salah alamat, saya pamit dulu..";

            await requestProvider.SendTextAsync(msgOut);
            await requestProvider.LeaveChat(chatId);
            return true;
        }

        public static ITelegramBotClient GetClient(string name)
        {
            return Bot.Clients[name];
        }

        public static async Task ClearLog()
        {
            const string logsPath = "Storage/Logs";
            var botClient = Bot.Client;
            var channelTarget = Bot.GlobalConfiguration["CommonConfig:ChannelLogs"].ToInt64();

            var dirInfo = new DirectoryInfo(logsPath);
            var files = dirInfo.GetFiles();
            var filteredFiles = files.Where(fileInfo => fileInfo.CreationTimeUtc < DateTime.UtcNow.AddDays(-1)).ToArray();

            if (filteredFiles.Length > 0)
            {
                Log.Information($"Found {filteredFiles.Length} of {files.Length}");
                foreach (var fileInfo in filteredFiles)
                {
                    var filePath = fileInfo.FullName;
                    Log.Information($"Uploading file {filePath}");
                    var fileStream = File.OpenRead(filePath);

                    var media = new InputOnlineFile(fileStream, fileInfo.Name);
                    await botClient.SendDocumentAsync(channelTarget, media);

                    filePath.DeleteFile();
                }
            }
            else
            {
                Log.Information("No Logs file need be processed for previous date");
            }
        }
    }
}