using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
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
            _settingsService = new SettingsService();
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;
            _chatProcessor = new ChatProcessor(context);

            ConsoleHelper.WriteLine("New Chat Members...");

            var settings = await _settingsService.GetSettingByGroup(msg.Chat.Id);
            var welcomeMessage = settings.Rows[0]["welcome_message"].ToString();
            var welcomeButton = settings.Rows[0]["welcome_button"].ToString();
            var welcomeMedia = settings.Rows[0]["welcome_media"].ToString();
            var welcomeMediaType = settings.Rows[0]["welcome_media_type"].ToString();

            var chatTitle = msg.Chat.Title;

            var newMembers = msg.NewChatMembers;
            var allNewMember = new StringBuilder();
            var noUsername = new StringBuilder();
            var newBots = new StringBuilder();
            var lastMember = newMembers.Last();

            foreach (var newMember in newMembers)
            {
//                var isCasBan = await _casBanProvider.IsCasBan(newMember.Id);

                var fullName = (newMember.FirstName + " " + newMember.LastName).Trim();
                var nameLink = MemberHelper.GetNameLink(newMember.Id, fullName);
                // if (newMembers.IndexOf(newMember) != newMembers.Length - 1)
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
                    noUsername.Append(nameLink);
                }

                if (newMember.IsBot)
                {
                    newBots.Append(nameLink);
                }
            }

            // var fromName = msg.From.FirstName;

//            var sendText = $"Hai {newMemberStr}" +
//                           $"\nSelamat datang di kontrakan <b>{chatTitle}</b>" +
//                           $"\nCount: {newMembers.Length}";

//            var sendText = welcomeMessage.Replace("{newMemberStr}", newMemberStr.ToString());

            var sendText = welcomeMessage.ResolveVariable(new
            {
                allNewMember,
                chatTitle
            });

            IReplyMarkup keyboard = null;
            if (welcomeButton != "")
            {
                keyboard = welcomeButton.ToReplyMarkup(2);
            }

            if (welcomeMediaType != "")
            {
                await _chatProcessor.SendMediaAsync(welcomeMedia, welcomeMediaType, sendText, keyboard);
            }
            else
            {
                await _chatProcessor.SendAsync(sendText, keyboard);
            }
            
        }
    }
}