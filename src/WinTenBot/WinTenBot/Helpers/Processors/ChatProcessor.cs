using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace WinTenBot.Helpers.Processors
{
    public class ChatProcessor
    {
        public ITelegramBotClient Client { get; set; }
        public Message Message { get; set; }
        public int SentMessageId { get; private set; }
        public int EditedMessageId { get; private set; }
        
        public int CallBackMessageId { get; set; }

        public ChatProcessor(IUpdateContext updateContext)
        {
            // ChatId = _updateContext.Update.Message.Chat.Id;
            Client = updateContext.Bot.Client;
            Message = updateContext.Update.CallbackQuery != null ? updateContext.Update.CallbackQuery.Message : updateContext.Update.Message;
        }

        public async Task SendAsync(string sendText, IReplyMarkup replyMarkup = null, bool replyToReplied = false)
        {
            var replyToMsgId = Message.MessageId;
            if (replyToReplied)
            {
                replyToMsgId = Message.ReplyToMessage.MessageId;
            }

            var date = Message.Date;
//            ConsoleHelper.WriteLine(date);
//            sendText += TimeHelper.Delay(_updateContext.Update.Message.Date);
            Message send = null;
            try
            {
                send = await Client.SendTextMessageAsync(
                    Message.Chat,
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
                    send = await Client.SendTextMessageAsync(
                        Message.Chat,
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
            IReplyMarkup replyMarkup = null)
        {
            switch (mediaType.ToLower())
            {
                case "document":
                    await Client.SendDocumentAsync(Message.Chat.Id, fileId, caption, ParseMode.Html,
                        replyMarkup: replyMarkup);
                    break;
            }
        }

        public async Task EditAsync(string sendText, InlineKeyboardMarkup replyMarkup = null)
        {
            Message edit = null;

            edit = await Client.EditMessageTextAsync(
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
                edit =await Client.EditMessageTextAsync(
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

        public async Task DeleteAsync(int messageId = -1)
        {
            var mssgId = messageId != -1 ? messageId : SentMessageId;

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

        public async Task<bool> IsAdminGroup()
        {
            var chatId = Message.Chat.Id;
            var userId = Message.From.Id;
            var isAdmin = false;

            var admins = await Client.GetChatAdministratorsAsync(chatId);
            foreach (var admin in admins)
            {
                if (userId == admin.User.Id)
                {
                    isAdmin = true;
                }
            }

            return isAdmin;
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
    }
}