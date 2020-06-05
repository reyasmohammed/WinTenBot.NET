using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using WinTenBot.Common;
using WinTenBot.IO;
using WinTenBot.Model;
using WinTenBot.Services;
using File = System.IO.File;

namespace WinTenBot.Telegram
{
    public static class Health
    {
        public static async Task<string> GetUrlStart(this TelegramService telegramService, string param)
        {
            var bot = await telegramService.Client.GetMeAsync();
            var username = bot.Username;
            return $"https://t.me/{username}?{param}";
        }

        public static async Task<User> GetMeAsync(this TelegramService telegramService)
        {
            return await telegramService.Client.GetMeAsync();
        }

        public static async Task<bool> IsBeta(this TelegramService telegramService)
        {
            var me = await GetMeAsync(telegramService);
            var isBeta = me.Username.ToLower().Contains("beta");
            Log.Information($"IsBeta: {isBeta}");
            return isBeta;
        }

        public static async Task<bool> IsBotAdded(this User[] users)
        {
            var me = await BotSettings.Client.GetMeAsync();
            return (from user in users where user.Id == me.Id select user.Id == me.Id).FirstOrDefault();
        }

        public static bool IsRestricted()
        {
            return BotSettings.GlobalConfiguration["CommonConfig:IsRestricted"].ToBool();
        }

        public static bool CheckRestriction(this long chatId)
        {
            try
            {
                var isRestricted = false;
                var globalRestrict = IsRestricted();
                var sudoers = BotSettings.GlobalConfiguration.GetSection("RestrictArea").Get<List<string>>();
                var match = sudoers.FirstOrDefault(x => x == chatId.ToString());

                Log.Information($@"Global Restriction: {globalRestrict}");
                if (match == null && globalRestrict)
                {
                    isRestricted = true;
                }

                Log.Information($"ChatId: {chatId} IsRestricted: {isRestricted}");
                return isRestricted;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Chat Restriction.");
                return false;
            }
        }

        public static async Task<bool> EnsureChatRestrictionAsync(this TelegramService telegramService)
        {
            Log.Information("Starting ensure Chat Restriction");

            var message = telegramService.MessageOrEdited;
            var chatId = message.Chat.Id;

            if (!chatId.CheckRestriction()) return false;

            Log.Information("I must leave right now!");
            var msgOut = $"Sepertinya saya salah alamat, saya pamit dulu..";

            await telegramService.SendTextAsync(msgOut);
            await telegramService.LeaveChat(chatId);
            return true;
        }

        public static ITelegramBotClient GetClient(string name)
        {
            return BotSettings.Clients[name];
        }
        
        public static async Task ClearLog()
        {
            try
            {
                const string logsPath = "Storage/Logs";
                var botClient = BotSettings.Client;
                var channelTarget = BotSettings.GlobalConfiguration["CommonConfig:ChannelLogs"].ToInt64();

                if (!channelTarget.ToString().StartsWith("-100"))
                {
                    Log.Information("Please specify ChannelTarget in appsettings.json");
                    return;
                }

                var dirInfo = new DirectoryInfo(logsPath);
                var files = dirInfo.GetFiles();
                var filteredFiles = files.Where(fileInfo =>
                    fileInfo.CreationTimeUtc < DateTime.UtcNow.AddDays(-1)).ToArray();

                if (filteredFiles.Length > 0)
                {
                    Log.Information($"Found {filteredFiles.Length} of {files.Length}");
                    foreach (var fileInfo in filteredFiles)
                    {
                        var filePath = fileInfo.FullName;
                        Log.Information($"Uploading file {filePath}");
                        await using var fileStream = File.OpenRead(filePath);

                        var media = new InputOnlineFile(fileStream, fileInfo.Name);
                        await botClient.SendDocumentAsync(channelTarget, media)
                            .ConfigureAwait(false);

                        fileStream.Close();
                        await fileStream.DisposeAsync().ConfigureAwait(false);
                        
                        filePath.DeleteFile();
                    }
                }
                else
                {
                    Log.Information("No Logs file need be processed for previous date");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Send .Log file to ChannelTarget.");
            }
        }

        public static async Task ClearLogs(this string logsPath, string filter = "", bool upload = true)
        {
            try
            {
                Log.Information($"Clearing {logsPath}, filter: {filter}, upload: {upload}");
                var botClient = BotSettings.Client;
                var channelTarget = BotSettings.BotChannelLogs;

                if (!channelTarget.ToString().StartsWith("-100"))
                {
                    Log.Information("Please specify ChannelTarget in appsettings.json");
                    return;
                }

                var dirInfo = new DirectoryInfo(logsPath);
                var files = dirInfo.GetFiles();
                var filteredFiles = files.Where(fileInfo =>
                    fileInfo.CreationTimeUtc < DateTime.UtcNow.AddDays(-1) &&
                    fileInfo.FullName.Contains(filter)).ToArray();

                if (filteredFiles.Length > 0)
                {
                    Log.Information($"Found {filteredFiles.Length} of {files.Length}");
                    foreach (var fileInfo in filteredFiles)
                    {
                        var filePath = fileInfo.FullName;
                        if (upload)
                        {
                            Log.Information($"Uploading file {filePath}");
                            var fileStream = File.OpenRead(filePath);

                            var media = new InputOnlineFile(fileStream, fileInfo.Name);
                            await botClient.SendDocumentAsync(channelTarget, media)
                                .ConfigureAwait(false);

                            fileStream.Close();
                            await fileStream.DisposeAsync().ConfigureAwait(false);
                        }

                        var old = filePath + ".old";
                        File.Move(filePath, old);
                        old.DeleteFile();
                    }
                }
                else
                {
                    Log.Information("No Logs file need be processed for previous date");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Send .Log file to ChannelTarget.");
            }
        }

        public static async Task EnsureChatHealthAsync(this TelegramService telegramService)
        {
            Log.Information("Ensuring chat health..");

            var message = telegramService.Message;
            var settingsService = new SettingsService
            {
                Message = message
            };

            var data = new Dictionary<string, object>()
            {
                ["chat_id"] = message.Chat.Id,
                ["chat_title"] = message.Chat.Title,
            };

            var update = await settingsService.SaveSettingsAsync(data);

            await settingsService.UpdateCache();
        }
    }
}