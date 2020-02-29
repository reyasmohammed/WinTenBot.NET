using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Types;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Helpers
{
    public static class MemberHelper
    {
        public static string GetNameLink(int userId, string name)
        {
            return $"<a href='tg://user?id={userId}'>{name}</a>";
        }

        public static string GetFromNameLink(this Message message)
        {
            var firstName = message.From.FirstName;
            var lastName = message.From.LastName;

            return $"<a href='tg://user?id={message.From.Id}'>{(firstName + " " + lastName).Trim()}</a>";
        }

        public static async Task<bool> IsAdminGroup(this TelegramProvider telegramProvider, Message message)
        {
            var chatId = message.Chat.Id;
            var userId = message.From.Id;
            var isAdmin = false;
            var client = telegramProvider.Client;

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

        // public static async Task<bool> IsBanInCache(this User user)
        // {
        //     var filtered = new DataTable(null);
        //     var data = await "fban_user.json".ReadCacheAsync();
        //     var userId = user.Id;
        //
        //     ConsoleHelper.WriteLine($"Checking {user} in Global Ban Cache");
        //     var search = data.AsEnumerable()
        //         .Where(row => row.Field<string>("user_id") == userId.ToString());
        //     if (search.Any())
        //     {
        //         filtered = search.CopyToDataTable();
        //     }
        //
        //     ConsoleHelper.WriteLine($"Caches found: {filtered.ToJson()}");
        //     return filtered.Rows.Count > 0;
        // }

        // public static async Task<bool> IsGBan(this User user)
        // {
        //     var query = await new Query("fban_user")
        //         .Where("user_id",user.Id)
        //         .ExecForSqLite(true)
        //         .GetAsync();
        //
        //     return query.Any();
        // }

        public static bool IsNoUsername(this User user)
        {
            return user.Username == null;
        }
        // public static bool IsSudoer(this RequestProvider requestProvider)
        // {
        //     var message = requestProvider.Message;
        //     return message.From.Id.IsSudoer();
        // }

        public static async Task CheckUsernameAsync(this TelegramProvider telegramProvider)
        {
            Log.Information("Starting check Username");

            var message = telegramProvider.MessageOrEdited;
            var fromUser = message.From;
            var noUsername = fromUser.IsNoUsername();
            Log.Information($"{fromUser} IsNoUsername: {noUsername}");

            if (noUsername)
            {
                var updateResult = await UpdateWarnUsernameStat(message);
                var updatedStep = updateResult.StepCount;

                await telegramProvider.SendTextAsync($"{fromUser} belum memasang username." +
                                                     $"\nPeringatan {updatedStep}/1000");
            }
        }

        public static async Task AfkCheckAsync(this TelegramProvider telegramProvider)
        {
            Log.Information("Starting check AFK");

            var afkService = new AfkService();
            var message = telegramProvider.MessageOrEdited;

            if (message.ReplyToMessage != null)
            {
                var repMsg = message.ReplyToMessage;
                var isAfkReply = await afkService.IsAfkAsync(repMsg);
                if (isAfkReply)
                    await telegramProvider.SendTextAsync($"{repMsg.GetFromNameLink()} sedang afk");
            }

            var isAfk = await afkService.IsAfkAsync(message);
            if (isAfk)
            {
                await telegramProvider.SendTextAsync($"{message.GetFromNameLink()} sudah tidak afk");

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

        public static async Task<bool> CheckGlobalBanAsync(this TelegramProvider telegramProvider,
            User userTarget = null)
        {
            Log.Information("Starting check Global Ban");

            var message = telegramProvider.MessageOrEdited;
            var user = message.From;

            if (userTarget != null) user = userTarget;

            var messageId = message.MessageId;

            var isBan = await user.Id.CheckGBan();
            Log.Information($"IsBan: {isBan}");
            if (isBan)
            {
                await telegramProvider.DeleteAsync(messageId);
                await telegramProvider.KickMemberAsync(user);
                await telegramProvider.UnbanMemberAsync(user);
            }

            return isBan;
        }

        public static async Task<bool> CheckCasBanAsync(this TelegramProvider telegramProvider)
        {
            bool isBan;
            Log.Information("Starting check in Cas Ban");

            var message = telegramProvider.MessageOrEdited;
            var user = message.From;
            isBan = await user.IsCasBanAsync();
            Log.Information($"{user} is CAS ban: {isBan}");
            if (isBan)
            {
                var sendText = $"{user} is banned in CAS!";
                await telegramProvider.SendTextAsync(sendText);
                await telegramProvider.KickMemberAsync(user);
                await telegramProvider.UnbanMemberAsync(user);
            }

            return isBan;
        }

        public static async Task<bool> CheckSpamWatchAsync(this TelegramProvider telegramProvider)
        {
            bool isBan;
            Log.Information("Starting Run SpamWatch");

            var message = telegramProvider.MessageOrEdited;
            var user = message.From;
            var spamWatch = await user.Id.CheckSpamWatch();
            isBan = spamWatch.IsBan;

            Log.Information($"{user} is SpamWatch Ban => {isBan}");

            if (isBan)
            {
                var sendText = $"{user} is banned in SpamWatch!" +
                               $"\nFed: @SpamWatch" +
                               $"\nReason: {spamWatch.Reason}";
                await telegramProvider.SendTextAsync(sendText);
                await telegramProvider.KickMemberAsync(user);
                await telegramProvider.UnbanMemberAsync(user);
            }

            return isBan;
        }

        private static async Task<WarnUsernameHistory> UpdateWarnUsernameStat(Message message)
        {
            var tableName = "warn_username_history";

            var data = new Dictionary<string, object>()
            {
                {"from_id", message.From.Id},
                {"first_name", message.From.FirstName},
                {"last_name", message.From.LastName},
                {"step_count", 1},
                {"chat_id", message.Chat.Id},
                {"created_at", DateTime.UtcNow}
            };

            var warnHistory = await new Query(tableName)
                .Where("from_id", data["from_id"])
                .ExecForSqLite(true)
                .GetAsync();

            var exist = warnHistory.Any();

            Log.Information($"Check Warn Username History: {exist}");

            if (exist)
            {
                var warnHistories = warnHistory.ToJson().MapObject<List<WarnUsernameHistory>>().First();

                Log.Information($"Mapped: {warnHistories.ToJson(true)}");

                var newStep = warnHistories.StepCount + 1;
                Log.Information($"New step for {message.From} is {newStep}");

                var update = new Dictionary<string, object>()
                {
                    {"step_count", newStep},
                    {"updated_at", DateTime.UtcNow}
                };

                var insertHit = await new Query(tableName)
                    .Where("from_id", data["from_id"])
                    .ExecForSqLite()
                    .UpdateAsync(update);

                Log.Information($"Update step: {insertHit}");
            }
            else
            {
                var insertHit = await new Query(tableName)
                    .ExecForSqLite()
                    .InsertAsync(data);

                Log.Information($"Insert Hit: {insertHit}");
            }

            var updatedHistory = await new Query(tableName)
                .Where("from_id", data["from_id"])
                .ExecForSqLite()
                .GetAsync();

            return updatedHistory.ToJson().MapObject<List<WarnUsernameHistory>>().First();
        }
    }
}