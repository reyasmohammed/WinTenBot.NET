using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using WinTenBot.Model;
using File = System.IO.File;

namespace WinTenBot.Helpers
{
    public static class BotHelper
    {
        public static async Task<string> GetUrlStart(this string param)
        {
            var bot = await Bot.Client.GetMeAsync();
            var username = bot.Username;
            return $"https://t.me/{username}?start={param}";
        }

        public static async Task<bool> IsBotAdded(this User[] users)
        {
            var me = await Bot.Client.GetMeAsync();
            return (from user in users where user.Id == me.Id select user.Id == me.Id).FirstOrDefault();
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