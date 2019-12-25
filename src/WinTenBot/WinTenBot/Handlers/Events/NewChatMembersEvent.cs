using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Events
{
    public class NewChatMembersEvent : IUpdateHandler
    {
        private CasBanProvider _casBanProvider;
        private SettingsService _settingsService;
        private ChatProcessor _chatProcessor;

        public NewChatMembersEvent()
        {
            _casBanProvider = new CasBanProvider();
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;
            _chatProcessor = new ChatProcessor(context);
            _settingsService = new SettingsService(msg.Chat);

            ConsoleHelper.WriteLine("New Chat Members...");

            var chatSettings = await _settingsService.GetMappedSettingsByGroup();

            var chatTitle = msg.Chat.Title;
            var memberCount = await _chatProcessor.GetMemberCount();
            var newMembers = msg.NewChatMembers;
            var newMemberCount = newMembers.Length;

            var parsedNewMember = await ParseMemberCategory(newMembers);
            var allNewMember = parsedNewMember.AllNewMember;
            var allNoUsername = parsedNewMember.AllNoUsername;
            var allNewBot = parsedNewMember.AllNewBot;

            if (chatSettings.WelcomeMessage == "")
            {
                chatSettings.WelcomeMessage = $"Hai {allNewMember}" +
                                              $"\nSelamat datang di kontrakan {chatTitle}";
            }
            
            var sendText = chatSettings.WelcomeMessage.ResolveVariable(new
            {
                allNewMember,
                allNoUsername,
                allNewBot,
                newMemberCount,
                chatTitle,
                memberCount
            });

            IReplyMarkup keyboard = null;
            if (chatSettings.WelcomeButton != "")
            {
                keyboard = chatSettings.WelcomeButton.ToReplyMarkup(2);
            }

            if (chatSettings.WelcomeMediaType != "")
            {
                await _chatProcessor.SendMediaAsync(
                    chatSettings.WelcomeMedia, 
                    chatSettings.WelcomeMediaType, 
                    sendText,
                    keyboard);
            }
            else
            {
                await _chatProcessor.SendAsync(sendText, keyboard);
            }
        }

        private async Task<NewMember> ParseMemberCategory(User[] users)
        {
            var lastMember = users.Last();
            var newMembers = new NewMember();
            var allNewMember = new StringBuilder();
            var allNoUsername = new StringBuilder();
            var allNewBot = new StringBuilder();

            ConsoleHelper.WriteLine($"Parsing {users.Length} members..");
            foreach (var newMember in users)
            {
                if (Bot.HostingEnvironment.IsProduction())
                {
                    var isCasBan = await _casBanProvider.IsCasBan(newMember.Id);
                }

                var fullName = (newMember.FirstName + " " + newMember.LastName).Trim();
                var nameLink = MemberHelper.GetNameLink(newMember.Id, fullName);

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
    }
}