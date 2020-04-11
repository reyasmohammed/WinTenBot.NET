using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
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

        public static string GetNameLink(this User user)
        {
            var firstName = user.FirstName;
            var lastName = user.LastName;

            return $"<a href='tg://user?id={user.Id}'>{(firstName + " " + lastName).Trim()}</a>";
        }

        public static string GetFromNameLink(this Message message)
        {
            var firstName = message.From.FirstName;
            var lastName = message.From.LastName;

            return $"<a href='tg://user?id={message.From.Id}'>{(firstName + " " + lastName).Trim()}</a>";
        }

        public static async Task AfkCheckAsync(this TelegramProvider telegramProvider)
        {
            Log.Information("Starting check AFK");

            var message = telegramProvider.MessageOrEdited;

            var settingService = new SettingsService(message);
            var chatSettings = await settingService.GetSettingByGroup();
            if (!chatSettings.EnableAfkStat)
            {
                Log.Information("Afk Stat is disabled in this Group!");
                return;
            }

            var afkService = new AfkService();
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

                var data = new Dictionary<string, object>
                {
                    {"chat_id", message.Chat.Id}, {"user_id", message.From.Id}, {"is_afk", 0}, {"afk_reason", ""}
                };

                await afkService.SaveAsync(data);
                await afkService.UpdateCacheAsync();
            }
        }

        public static async Task CheckMataZiziAsync(this TelegramProvider telegramProvider)
        {
            try
            {
                var message = telegramProvider.Message;
                var fromId = message.From.Id;
                var fromUsername = message.From.Username;
                var fromFName = message.From.FirstName;
                var fromLName = message.From.LastName;
                var chatId = message.Chat.Id;

                Log.Information("Starting SangMata check..");

                var query = await new Query("hit_activity")
                    .ExecForMysql(true)
                    .Where("from_id", fromId)
                    .Where("chat_id", chatId)
                    .OrderByDesc("timestamp")
                    .Limit(1)
                    .GetAsync();

                if (!query.Any())
                {
                    Log.Information($"This may first Hit from User {fromId}");
                    return;
                }

                var hitActivity = query.ToJson().MapObject<List<HitActivity>>().FirstOrDefault();

                Log.Information($"SangMata: {hitActivity.ToJson(true)}");

                var changesCount = 0;
                var msgBuild = new StringBuilder();

                msgBuild.AppendLine("😽 <b>MataZizi</b>");
                msgBuild.AppendLine($"<b>UserID:</b> {fromId}");

                if (fromUsername != hitActivity.FromUsername)
                {
                    Log.Information("Username changed detected!");
                    msgBuild.AppendLine($"Mengubah Username menjadi @{fromUsername}");
                    changesCount++;
                }
                
                if (fromFName != hitActivity.FromFirstName)
                {
                    Log.Information("First Name changed detected!");
                    msgBuild.AppendLine($"Mengubah nama depan menjadi {fromFName}");
                    changesCount++;
                }
                
                if (fromLName != hitActivity.FromLastName)
                {
                    Log.Information("Last Name changed detected!");
                    msgBuild.AppendLine($"Mengubah nama belakang menjadi {fromLName}");
                    changesCount++;
                }

                if(changesCount>0)
                    await telegramProvider.SendTextAsync(msgBuild.ToString().Trim());

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error SangMata");
            }
        }

        #region AntiSpam

        public static async Task<bool> CheckGlobalBanAsync(this TelegramProvider telegramProvider,
            User userTarget = null)
        {
            Log.Information("Starting check Global Ban");

            var message = telegramProvider.MessageOrEdited;
            var user = message.From;

            var settingService = new SettingsService(message);
            var chatSettings = await settingService.GetSettingByGroup();
            if (!chatSettings.EnableFedEs2)
            {
                Log.Information("Fed ES2 Ban is disabled in this Group!");
                return false;
            }

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

            var settingService = new SettingsService(message);
            var chatSettings = await settingService.GetSettingByGroup();
            if (!chatSettings.EnableFedCasBan)
            {
                Log.Information("Fed Cas Ban is disabled in this Group!");
                return false;
            }

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
            var settingService = new SettingsService(message);
            var chatSettings = await settingService.GetSettingByGroup();
            if (!chatSettings.EnableFedSpamWatch)
            {
                Log.Information("Fed SpamWatch is disabled in this Group!");
                return false;
            }

            var user = message.From;
            var spamWatch = await user.Id.CheckSpamWatch();
            isBan = spamWatch.IsBan;

            Log.Information($"{user} is SpamWatch Ban => {isBan}");

            if (isBan)
            {
                var sendText = $"{user} is banned in SpamWatch!" +
                               "\nFed: @SpamWatch" +
                               $"\nReason: {spamWatch.Reason}";
                await telegramProvider.SendTextAsync(sendText);
                await telegramProvider.KickMemberAsync(user);
                await telegramProvider.UnbanMemberAsync(user);
            }

            return isBan;
        }

        #endregion

        #region Manual Warn Member

        public static async Task WarnMemberAsync(this TelegramProvider telegramProvider)
        {
            try
            {
                Log.Information("Prepare Warning Member..");
                var message = telegramProvider.Message;
                var repMessage = message.ReplyToMessage;
                var textMsg = message.Text;
                var fromId = message.From.Id;
                var partText = textMsg.Split(" ");
                var reasonWarn = partText.ValueOfIndex(1) ?? "no-reason";
                var user = repMessage.From;
                Log.Information($"Warning User: {user}");

                var warnLimit = 4;
                var warnHistory = await UpdateWarnMemberStat(message);
                var updatedStep = warnHistory.StepCount;
                var lastMessageId = warnHistory.LastWarnMessageId;
                var nameLink = user.GetNameLink();

                var sendText = $"{nameLink} di beri peringatan!." +
                               $"\nPeringatan ke {updatedStep} dari {warnLimit}";

                if (updatedStep == warnLimit) sendText += "\nIni peringatan terakhir!";

                if (!reasonWarn.IsNullOrEmpty())
                {
                    sendText += $"\n<b>Reason:</b> {reasonWarn}";
                }

                if (updatedStep > warnLimit)
                {
                    var sendWarn = $"Batas peringatan telah di lampaui." +
                                   $"\n{nameLink} di tendang sekarang!";
                    await telegramProvider.SendTextAsync(sendWarn);

                    await telegramProvider.KickMemberAsync(user);
                    await telegramProvider.UnbanMemberAsync(user);
                    await ResetWarnMemberStatAsync(message);

                    return;
                }

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Hapus peringatan", $"action remove-warn {user.Id}"),
                    }
                });

                await telegramProvider.SendTextAsync(sendText, inlineKeyboard);
                await message.UpdateLastWarnMemberMessageIdAsync(telegramProvider.SentMessageId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Warn Member");
            }
        }

        private static async Task<WarnMemberHistory> UpdateWarnMemberStat(Message message)
        {
            var tableName = "warn_member_history";
            var repMessage = message.ReplyToMessage;
            var textMsg = message.Text;
            var partText = textMsg.Split(" ");
            var reasonWarn = partText.ValueOfIndex(1) ?? "no-reason";

            var chatId = repMessage.Chat.Id;
            var fromId = repMessage.From.Id;
            var fromFName = repMessage.From.FirstName;
            var fromLName = repMessage.From.LastName;
            var warnerId = message.From.Id;
            var warnerFName = message.From.FirstName;
            var warnerLName = message.From.LastName;

            var warnHistory = await new Query(tableName)
                .Where("from_id", fromId)
                .Where("chat_id",chatId)
                .ExecForSqLite(true)
                .GetAsync();

            var exist = warnHistory.Any();

            Log.Information($"Check Warn Username History: {exist}");

            if (exist)
            {
                var warnHistories = warnHistory.ToJson().MapObject<List<WarnMemberHistory>>().First();

                Log.Information($"Mapped: {warnHistories.ToJson(true)}");

                var newStep = warnHistories.StepCount + 1;
                Log.Information($"New step for {message.From} is {newStep}");

                var update = new Dictionary<string, object>
                {
                    {"first_name", fromFName},
                    {"last_name", fromLName},
                    {"step_count", newStep},
                    {"reason_warn", reasonWarn},
                    {"warner_first_name", warnerFName},
                    {"warner_last_name", warnerLName},
                    {"updated_at", DateTime.UtcNow}
                };

                var insertHit = await new Query(tableName)
                    .Where("from_id", fromId)
                    .Where("chat_id",chatId)
                    .ExecForSqLite(true)
                    .UpdateAsync(update);

                Log.Information($"Update step: {insertHit}");
            }
            else
            {
                var data = new Dictionary<string, object>
                {
                    {"from_id", fromId},
                    {"first_name", fromFName},
                    {"last_name", fromLName},
                    {"step_count", 1},
                    {"reason_warn", reasonWarn},
                    {"warner_user_id", warnerId},
                    {"warner_first_name", warnerFName},
                    {"warner_last_name", warnerLName},
                    {"chat_id", message.Chat.Id},
                    {"created_at", DateTime.UtcNow}
                };

                var insertHit = await new Query(tableName)
                    .ExecForSqLite(true)
                    .InsertAsync(data);

                Log.Information($"Insert Hit: {insertHit}");
            }

            var updatedHistory = await new Query(tableName)
                .Where("from_id", fromId)
                .Where("chat_id",chatId)
                .ExecForSqLite(true)
                .GetAsync();

            return updatedHistory.ToJson().MapObject<List<WarnMemberHistory>>().First();
        }

        public static async Task UpdateLastWarnMemberMessageIdAsync(this Message message, long messageId)
        {
            Log.Information("Updating last Warn Member MessageId.");

            var tableName = "warn_member_history";
            var fromId = message.ReplyToMessage.From.Id;
            var chatId = message.Chat.Id;
            
            var update = new Dictionary<string, object>
            {
                {"last_warn_message_id", messageId},
                {"updated_at", DateTime.UtcNow}
            };

            var insertHit = await new Query(tableName)
                .Where("from_id", fromId)
                .Where("chat_id",chatId)
                .ExecForSqLite(true)
                .UpdateAsync(update);

            Log.Information($"Update lastWarn: {insertHit}");
        }

        public static async Task ResetWarnMemberStatAsync(Message message)
        {
            Log.Information("Resetting warn Username step.");

            var tableName = "warn_member_history";
            var fromId = message.ReplyToMessage.From.Id;
            var chatId = message.Chat.Id;

            var update = new Dictionary<string, object>
            {
                {"step_count", 0},
                {"updated_at", DateTime.UtcNow}
            };

            var insertHit = await new Query(tableName)
                .Where("from_id", fromId)
                .Where("chat_id",chatId)
                .ExecForSqLite(true)
                .UpdateAsync(update);

            Log.Information($"Update step: {insertHit}");
        }

        public static async Task RemoveWarnMemberStatAsync(this TelegramProvider telegramProvider, int userId)
        {
            Log.Information("Removing warn Member stat.");

            var tableName = "warn_member_history";
            var message = telegramProvider.Message;
            var chatId = message.Chat.Id;

            var update = new Dictionary<string, object>
            {
                {"step_count", 0},
                {"updated_at", DateTime.UtcNow}
            };

            var insertHit = await new Query(tableName)
                .Where("from_id", userId)
                .Where("chat_id",chatId)
                .ExecForSqLite(true)
                .UpdateAsync(update);

            Log.Information($"Update step: {insertHit}");
        }

        #endregion

        #region Check Username

        public static bool IsNoUsername(this User user)
        {
            return user.Username == null;
        }

        public static async Task CheckUsernameAsync(this TelegramProvider telegramProvider)
        {
            try
            {
                Log.Information("Starting check Username");

                var warnLimit = 4;
                var message = telegramProvider.MessageOrEdited;
                var fromUser = message.From;
                var nameLink = fromUser.GetNameLink();

                var settingService = new SettingsService(message);
                var chatSettings = await settingService.GetSettingByGroup();
                if (!chatSettings.EnableWarnUsername)
                {
                    Log.Information("Warn Username is disabled in this Group!");
                    return;
                }

                var noUsername = fromUser.IsNoUsername();
                Log.Information($"{fromUser} IsNoUsername: {noUsername}");

                if (noUsername)
                {
                    var updateResult = await UpdateWarnUsernameStat(message);
                    var updatedStep = updateResult.StepCount;
                    var lastMessageId = updateResult.LastWarnMessageId;

                    await telegramProvider.DeleteAsync(lastMessageId);

                    var sendText = $"Hai {nameLink}, kamu belum memasang username!" +
                                   $"\nPeringatan ke {updatedStep} dari {warnLimit}";

                    if (updatedStep == warnLimit) sendText += "\n\n<b>Ini peringatan terakhir!</b>";

                    if (updatedStep > warnLimit)
                    {
                        var sendWarn = $"Batas peringatan telah di lampaui." +
                                       $"\n{nameLink} di tendang sekarang!";
                        await telegramProvider.SendTextAsync(sendWarn);

                        await telegramProvider.KickMemberAsync(fromUser);
                        await telegramProvider.UnbanMemberAsync(fromUser);
                        await ResetWarnUsernameStatAsync(message);

                        return;
                    }

                    var keyboard = new InlineKeyboardMarkup(
                        InlineKeyboardButton.WithUrl("Cara Pasang Username", "https://t.me/WinTenDev/29")
                    );

                    await telegramProvider.SendTextAsync(sendText, keyboard);
                    await message.UpdateLastWarnUsernameMessageIdAsync(telegramProvider.SentMessageId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error check Username");
            }
        }

        private static async Task<WarnUsernameHistory> UpdateWarnUsernameStat(Message message)
        {
            var tableName = "warn_username_history";

            var data = new Dictionary<string, object>
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
                .Where("chat_id", data["chat_id"])
                .ExecForMysql(true)
                .GetAsync();

            var exist = warnHistory.Any();

            Log.Information($"Check Warn Username History: {exist}");

            if (exist)
            {
                var warnHistories = warnHistory.ToJson().MapObject<List<WarnUsernameHistory>>().First();

                Log.Information($"Mapped: {warnHistories.ToJson(true)}");

                var newStep = warnHistories.StepCount + 1;
                Log.Information($"New step for {message.From} is {newStep}");

                var update = new Dictionary<string, object>
                {
                    {"step_count", newStep}, {"updated_at", DateTime.UtcNow}
                };

                var insertHit = await new Query(tableName)
                    .Where("from_id", data["from_id"])
                    .Where("chat_id", data["chat_id"])
                    .ExecForMysql(true)
                    .UpdateAsync(update);

                Log.Information($"Update step: {insertHit}");
            }
            else
            {
                var insertHit = await new Query(tableName)
                    .ExecForMysql(true)
                    .InsertAsync(data);

                Log.Information($"Insert Hit: {insertHit}");
            }

            var updatedHistory = await new Query(tableName)
                .Where("from_id", data["from_id"])
                .Where("chat_id", data["chat_id"])
                .ExecForMysql(true)
                .GetAsync();

            return updatedHistory.ToJson().MapObject<List<WarnUsernameHistory>>().First();
        }

        public static async Task ResetWarnUsernameStatAsync(Message message)
        {
            Log.Information("Resetting warn Username step.");

            var tableName = "warn_username_history";
            var fromId = message.From.Id;
            var chatId = message.Chat.Id;

            var update = new Dictionary<string, object>
            {
                {"step_count", 0}, 
                {"updated_at", DateTime.UtcNow}
            };

            var insertHit = await new Query(tableName)
                .Where("from_id", fromId)
                .Where("chat_id", chatId)
                .ExecForMysql(true)
                .UpdateAsync(update);

            Log.Information($"Update step: {insertHit}");
        }

        public static async Task UpdateLastWarnUsernameMessageIdAsync(this Message message, long messageId)
        {
            Log.Information("Updating last Warn MessageId.");

            var tableName = "warn_username_history";
            var fromId = message.From.Id;
            var chatId = message.Chat.Id;

            var update = new Dictionary<string, object>
            {
                {"last_warn_message_id", messageId},
                {"updated_at", DateTime.UtcNow}
            };

            var insertHit = await new Query(tableName)
                .Where("from_id", fromId)
                .Where("chat_id", chatId)
                .ExecForMysql(true)
                .UpdateAsync(update);

            Log.Information($"Update lastWarn: {insertHit}");
        }

        #endregion
    }
}