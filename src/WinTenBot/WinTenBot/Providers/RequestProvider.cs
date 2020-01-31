using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Model;

namespace WinTenBot.Providers
{
    public class RequestProvider
    {
        private IUpdateContext UpdateContext { get; set; }
        private string AppendText { get; set; }
        public ITelegramBotClient Client { get; set; }
        public Message Message { get; set; }
        public int SentMessageId { get; private set; }
        public int EditedMessageId { get; private set; }
        public int CallBackMessageId { get; set; }
        private string TimeInit { get; set; }
        private string TimeProc { get; set; }

        public RequestProvider(IUpdateContext updateContext)
        {
            UpdateContext = updateContext;
            Client = updateContext.Bot.Client;
            Message = updateContext.Update.CallbackQuery != null
                ? updateContext.Update.CallbackQuery.Message
                : updateContext.Update.Message;
            if (Message != null)
            {
                TimeInit = Message.Date.GetDelay();
            }
        }


        #region Message

        public async Task SendTextAsync(string sendText, InlineKeyboardMarkup replyMarkup = null,
            int replyToMsgId = -1, long customChatId = -1)
        {
            TimeProc = Message.Date.GetDelay();

            if (sendText != "")
            {
                sendText += $"\n\n⏱ <code>{TimeInit} s</code> | ⌛️ <code>{TimeProc} s</code>";
            }

            var chatTarget = Message.Chat.Id;
            if (customChatId < -1)
            {
                chatTarget = customChatId;
            }

            Message send = null;
            try
            {
                ConsoleHelper.WriteLine($"Sending message to {chatTarget}");
                send = await Client.SendTextMessageAsync(
                    chatTarget,
                    sendText,
                    ParseMode.Html,
                    replyMarkup: replyMarkup,
                    replyToMessageId: replyToMsgId
                );
            }
            catch (ApiRequestException apiRequestException)
            {
                ConsoleHelper.WriteLine($"SendMessage: {apiRequestException.Message}");

                try
                {
                    ConsoleHelper.WriteLine($"Try Sending message to {chatTarget} without reply to Msg Id.");
                    send = await Client.SendTextMessageAsync(
                        chatTarget,
                        sendText,
                        ParseMode.Html,
                        replyMarkup: replyMarkup
                    );
                }
                catch (ApiRequestException apiRequestException2)
                {
                    ConsoleHelper.WriteLine(
                        $"SendMessage: {apiRequestException2.ErrorCode}: {apiRequestException2.Message}");
                }
            }

            if (send != null) SentMessageId = send.MessageId;
        }

        public async Task SendMediaAsync(string fileId, string mediaType, string caption = "",
            IReplyMarkup replyMarkup = null, int replyToMsgId = -1)
        {
            ConsoleHelper.WriteLine($"Sending media: {mediaType}, fileId: {fileId} to {Message.Chat.Id}");
            switch (mediaType.ToLower())
            {
                case "document":
                    await Client.SendDocumentAsync(Message.Chat.Id, fileId, caption, ParseMode.Html,
                        replyMarkup: replyMarkup, replyToMessageId: replyToMsgId);
                    break;

                case "photo":
                    await Client.SendPhotoAsync(Message.Chat.Id, fileId, caption, ParseMode.Html,
                        replyMarkup: replyMarkup, replyToMessageId: replyToMsgId);
                    break;

                default:
                    ConsoleHelper.WriteLine($"Media unknown: {mediaType}");
                    break;
            }
        }

        public async Task EditAsync(string sendText, InlineKeyboardMarkup replyMarkup = null)
        {
            TimeProc = Message.Date.GetDelay();

            if (sendText != "")
            {
                sendText += $"\n\n⏱ <code>{TimeInit} s</code> | ⌛️ <code>{TimeProc} s</code>";
            }

            var edit = await Client.EditMessageTextAsync(
                Message.Chat,
                SentMessageId,
                sendText,
                ParseMode.Html,
                replyMarkup: replyMarkup
            );

            EditedMessageId = edit.MessageId;
        }

        public async Task EditMessageCallback(string sendText, InlineKeyboardMarkup replyMarkup = null)
        {
            Message edit = null;
            try
            {
                ConsoleHelper.WriteLine($"Editing {CallBackMessageId}");
                edit = await Client.EditMessageTextAsync(
                    Message.Chat,
                    CallBackMessageId,
                    sendText,
                    ParseMode.Html,
                    replyMarkup: replyMarkup
                );
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteLine(e);
            }
        }

        public async Task AppendTextAsync(string sendText, InlineKeyboardMarkup replyMarkup = null)
        {
            if (string.IsNullOrEmpty(AppendText))
            {
                ConsoleHelper.WriteLine("Sending new message");
                AppendText = sendText;
                await SendTextAsync(AppendText, replyMarkup);
            }
            else
            {
                ConsoleHelper.WriteLine("Next, edit existing message");
                AppendText += $"\n{sendText}";
                await EditAsync(AppendText, replyMarkup);
            }
        }

        public async Task DeleteAsync(int messageId = -1, int delay = 0)
        {
            var mssgId = messageId != -1 ? messageId : SentMessageId;
            Thread.Sleep(delay);

            try
            {
                ConsoleHelper.WriteLine($"Delete MsgId: {mssgId} on ChatId: {Message.Chat.Id}");
                await Client.DeleteMessageAsync(Message.Chat.Id, mssgId);
            }
            catch (ChatNotFoundException chatNotFoundException)
            {
                ConsoleHelper.WriteLine($"{chatNotFoundException.ErrorCode}: {chatNotFoundException.Message}");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine($"{ex.Message}");
            }
        }

        #endregion Message

        public async Task<bool> IsAdminGroup()
        {
            var chatId = Message.Chat.Id;
            var userId = Message.From.Id;
            var isAdmin = false;

            if (!IsPrivateChat())
            {
                var admins = await Client.GetChatAdministratorsAsync(chatId);
                foreach (var admin in admins)
                {
                    if (userId == admin.User.Id)
                    {
                        isAdmin = true;
                    }
                }
            }

            return isAdmin;
        }

        public async Task<bool> IsAdminOrPrivateChat()
        {
            var isAdmin = await IsAdminGroup();
            var isPrivateChat = IsPrivateChat();

            return isAdmin || isPrivateChat;
        }

        public bool IsSudoer()
        {
            return Message.From.Id.IsSudoer();
        }

        public bool IsPrivateChat()
        {
            return Message.Chat.Type == ChatType.Private;
        }

        public async Task<string> GetMentionAdminsStr()
        {
            var admins = await Client.GetChatAdministratorsAsync(Message.Chat.Id);
            var adminStr = string.Empty;
            foreach (var admin in admins)
            {
                var user = admin.User;
                var nameLink = MemberHelper.GetNameLink(user.Id, "&#8203;");

                adminStr += $"{nameLink}";
            }

            return adminStr;
        }

        #region Member Exec

        public async Task<bool> KickMemberAsync(User user = null)
        {
            var isKicked = false;
            var idTarget = user.Id;
            var fromId = Message.From.Id;
            //if(id == -1)
            //{
            //    idTarget = Message.From.Id;
            //}
            ConsoleHelper.WriteLine($"Kick {idTarget} from {Message.Chat.Id}");
            try
            {
                await Client.KickChatMemberAsync(Message.Chat.Id, idTarget, DateTime.Now);
                isKicked = true;
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine("KickMember " + ex.Message);
                ConsoleHelper.WriteLine(ex.StackTrace);
                // await SendAsync(ex.Message);
                isKicked = false;
            }

            return isKicked;
        }

        public async Task UnbanMemberAsync(User user = null)
        {
            var idTarget = user.Id;
            ConsoleHelper.WriteLine($"Unban {idTarget} from {Message.Chat.Id}");
            try
            {
                await Client.UnbanChatMemberAsync(Message.Chat.Id, idTarget);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine(ex.Message);
                ConsoleHelper.WriteLine(ex.StackTrace);
                await SendTextAsync(ex.Message);
            }
        }

        public async Task<RequestResult> PromoteChatMemberAsync(int userId)
        {
            var requestResult = new RequestResult();
            try
            {
                await Client.PromoteChatMemberAsync(
                    Message.Chat.Id,
                    userId,
                    canChangeInfo: false,
                    canPostMessages: false,
                    canEditMessages: false,
                    canDeleteMessages: true,
                    canInviteUsers: true,
                    canRestrictMembers: true,
                    canPinMessages: true);

                requestResult.IsSuccess = true;
            }
            catch (ApiRequestException apiRequestException)
            {
                requestResult.IsSuccess = false;
                requestResult.ErrorCode = apiRequestException.ErrorCode;
                requestResult.ErrorMessage = apiRequestException.Message;
            }

            return requestResult;
        }

        public async Task<RequestResult> DemoteChatMemberAsync(int userId)
        {
            var requestResult = new RequestResult();
            try
            {
                await Client.PromoteChatMemberAsync(
                    Message.Chat.Id,
                    userId,
                    canChangeInfo: false,
                    canPostMessages: false,
                    canEditMessages: false,
                    canDeleteMessages: false,
                    canInviteUsers: false,
                    canRestrictMembers: false,
                    canPinMessages: false);

                requestResult.IsSuccess = true;
            }
            catch (ApiRequestException apiRequestException)
            {
                requestResult.IsSuccess = false;
                requestResult.ErrorCode = apiRequestException.ErrorCode;
                requestResult.ErrorMessage = apiRequestException.Message;
                ConsoleHelper.WriteLine(apiRequestException.ToString());
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine(ex.ToString());
            }

            return requestResult;
        }

        #endregion

        public async Task LeaveChat(long chatId = 0)
        {
            var chatTarget = chatId;
            if (chatId == 0)
            {
                chatTarget = Message.Chat.Id;
            }

            ConsoleHelper.WriteLine($"Leaving from {chatTarget}");
            await Client.LeaveChatAsync(chatTarget);
        }

        public async Task<long> GetMemberCount()
        {
            var member = await Client.GetChatMembersCountAsync(Message.Chat.Id);
            return member;
        }

        public async Task<Chat> GetChat()
        {
            var chat = await Bot.Client.GetChatAsync(Message.Chat.Id);
            return chat;
        }
    }
}