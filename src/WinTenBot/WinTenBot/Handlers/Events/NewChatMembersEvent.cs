using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Enums;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Events
{
    public class NewChatMembersEvent : IUpdateHandler
    {
        private ElasticSecurityService _elasticSecurityService;
        private SettingsService _settingsService;
        private TelegramProvider _telegramProvider;
        private ChatSetting Settings { get; set; }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;
            _telegramProvider = new TelegramProvider(context);
            _settingsService = new SettingsService(msg);
            _elasticSecurityService = new ElasticSecurityService(context.Update.Message);

            Log.Information("New Chat Members...");
            
            var chatSettings = await _settingsService.ReadCache();
            Settings = chatSettings;
            
            if(!chatSettings.EnableWelcomeMessage){
                Log.Information("Welcome message is disabled!");
                return;
            }
            
            var newMembers = msg.NewChatMembers;
            var isBootAdded = await newMembers.IsBotAdded();
            if (isBootAdded)
            {
                var isRestricted = await _telegramProvider.EnsureChatRestrictionAsync();
                if (isRestricted) return;

                var botName = BotSettings.GlobalConfiguration["Engines:ProductName"];
                var sendText = $"Hai, perkenalkan saya {botName}" +
                               $"\nFYI saya di bangun ulang menggunakan .NET Core, tepatnya ASP .NET Core." +
                               $"\n\nAku adalah bot pendebug dan grup manajemen yang di lengkapi dengan alat keamanan. " +
                               $"Agar saya berfungsi penuh, jadikan saya admin dengan level standard. " +
                               $"\n\nAku akan menerapkan konfigurasi standard jika aku baru pertama kali masuk kesini. " +
                               $"\n\nUntuk melihat daftar perintah bisa ketikkan /help";

                await _telegramProvider.SendTextAsync(sendText);
                await _settingsService.SaveSettingsAsync(new Dictionary<string, object>()
                {
                    {"chat_id", msg.Chat.Id},
                    {"chat_title", msg.Chat.Title}
                });

                if (newMembers.Length == 1) return;
            }

            var parsedNewMember = await ParseMemberCategory(newMembers);
            var allNewMember = parsedNewMember.AllNewMember;
            var allNoUsername = parsedNewMember.AllNoUsername;
            var allNewBot = parsedNewMember.AllNewBot;

            if (allNewMember.Length > 0)
            {

                var chatTitle = msg.Chat.Title;
                var greet = TimeHelper.GetTimeGreet();
                var memberCount = await _telegramProvider.GetMemberCount();
                var newMemberCount = newMembers.Length;

                Log.Information("Preparing send Welcome..");

                if (chatSettings.WelcomeMessage.IsNullOrEmpty())
                {
                    chatSettings.WelcomeMessage = $"Hai {allNewMember}" +
                                                  $"\nSelamat datang di kontrakan {chatTitle}" +
                                                  $"\nKamu adalah anggota ke-{memberCount}";
                }

                var sendText = chatSettings.WelcomeMessage.ResolveVariable(new
                {
                    allNewMember,
                    allNoUsername,
                    allNewBot,
                    chatTitle,
                    greet,
                    newMemberCount,
                    memberCount
                });

                InlineKeyboardMarkup keyboard = null;
                if (!chatSettings.WelcomeButton.IsNullOrEmpty())
                {
                    keyboard = chatSettings.WelcomeButton.ToReplyMarkup(2);
                }

                if(chatSettings.EnableHumanVerification){
                    Log.Information("Human verification is enabled!");
                    Log.Information("Adding verify button..");
                    
                    var userId = newMembers[0].Id;
                    var verifyButton = $"Saya Manusia!|verify {userId}";

                    var withVerifyArr = new string [] { chatSettings.WelcomeButton, verifyButton };
                    var withVerify = string.Join(",", withVerifyArr);

                    keyboard = withVerify.ToReplyMarkup(2);
                }

                var prevMsgId = chatSettings.LastWelcomeMessageId.ToInt();


                int sentMsgId = -1;

                if (chatSettings.WelcomeMediaType != MediaType.Unknown)
                {
                    var mediaType = (MediaType) chatSettings.WelcomeMediaType;
                    sentMsgId = (await _telegramProvider.SendMediaAsync(
                        chatSettings.WelcomeMedia,
                        mediaType,
                        sendText,
                        keyboard)).MessageId;
                }
                else
                {
                    sentMsgId = (await _telegramProvider.SendTextAsync(sendText, keyboard)).MessageId;
                }

                if (!chatSettings.EnableHumanVerification)
                {
                    await _telegramProvider.DeleteAsync(prevMsgId);
                }
                
                await _settingsService.SaveSettingsAsync(new Dictionary<string, object>()
                {
                    {"chat_id", msg.Chat.Id},
                    {"chat_title", msg.Chat.Title},
                    {"members_count", memberCount},
                    {"last_welcome_message_id", sentMsgId}
                });

                await _settingsService.UpdateCache();
            }
            else
            {
                Log.Information("Welcome Message ignored because User is Global Banned.");
            }
        }

        private async Task<NewMember> ParseMemberCategory(User[] users)
        {
            var lastMember = users.Last();
            var newMembers = new NewMember();
            var allNewMember = new StringBuilder();
            var allNoUsername = new StringBuilder();
            var allNewBot = new StringBuilder();

            Log.Information($"Parsing new {users.Length} members..");
            foreach (var newMember in users)
            {
                var newMemberId = newMember.Id;
                
                if (Settings.EnableHumanVerification)
                {
                    Log.Information($"Restricting {newMemberId}");
                    await _telegramProvider.RestrictMemberAsync(newMemberId);
                }
                
                var isBan = await _telegramProvider.CheckGlobalBanAsync(newMember);
                if (isBan) continue;

                if (BotSettings.HostingEnvironment.IsProduction())
                {
                    // var isCasBan = await IsCasBan(newMember.Id);
                    await newMember.IsCasBanAsync();
                }

                var fullName = (newMember.FirstName + " " + newMember.LastName).Trim();
                var nameLink = MemberHelper.GetNameLink(newMemberId, fullName);

                if (newMember != lastMember)
                {
                    allNewMember.Append(nameLink + ", ");
                }
                else
                {
                    allNewMember.Append(nameLink);
                }

                if (newMember.Username == "")
                {
                    allNoUsername.Append(nameLink);
                }

                if (newMember.IsBot)
                {
                    allNewBot.Append(nameLink);
                }
            }

            newMembers.AllNewMember = allNewMember;
            newMembers.AllNoUsername = allNoUsername;
            newMembers.AllNewBot = allNewBot;

            return newMembers;
        }

        private async Task<bool> CheckGlobalBanAsync(User user)
        {
            var userId = user.Id;
            var isKicked = false;

            // var isBan = await _elasticSecurityService.IsExistInCache(userId);
            var isBan = await user.Id.CheckGBan();
            Log.Information($"{user} IsBan: {isBan}");
            if (!isBan) return isKicked;

            var sendText = $"{user} terdeteksi pada penjaringan WinTenDev ES2 tapi gagal di tendang.";
            isKicked = await _telegramProvider.KickMemberAsync(user);
            if (isKicked)
            {
                await _telegramProvider.UnbanMemberAsync(user);
                sendText = sendText.Replace("tapi gagal", "dan berhasil");
            }
            else
            {
                sendText += " Pastikan saya admin yang dapat menghapus Pengguna";
            }

            await _telegramProvider.SendTextAsync(sendText);

            return isKicked;
        }
    }
}