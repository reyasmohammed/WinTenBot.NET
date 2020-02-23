using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Types;
using WinTenBot.Providers;

namespace WinTenBot.Helpers
{
    public static class ActivityHelper
    {
        public static async Task HitActivity(this RequestProvider requestProvider)
        {
            Log.Information("Starting Hit Activity");

            var message = requestProvider.Message;
            var botUser = await requestProvider.GetBotUser();
            var data = new Dictionary<string, object>()
            {
                {"via_bot", botUser.Username},
                {"message_type", message.Type.ToString()},
                {"from_id", message.From.Id},
                {"from_first_name", message.From.FirstName},
                {"from_last_name", message.From.LastName},
                {"from_username", message.From.Username},
                {"from_lang_code", message.From.LanguageCode},
                {"chat_id", message.Chat.Id},
                {"chat_username", message.Chat.Username},
                {"chat_type", message.Chat.Type.ToString()},
                {"chat_title", message.Chat.Title},
            };

            var insertHit = await new Query("hit_activity")
                .ExecForMysql()
                .InsertAsync(data);

            Log.Information($"Insert Hit: {insertHit}");
        }

        public static void HitActivityBackground(this RequestProvider requestProvider)
        {
            var message = requestProvider.Message;
            BackgroundJob.Enqueue(() => HitActivity(requestProvider));

            Log.Information("Hit Activity scheduled in Background");
        }
    }
}