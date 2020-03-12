using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Enums;
using WinTenBot.Helpers;
using WinTenBot.Model;
using File = System.IO.File;

namespace WinTenBot.Providers
{
    public class TelegramProvider
    {
        public TelegramProvider(IUpdateContext updateContext)
        {
            Context = updateContext;
            Client = updateContext.Bot.Client;
            Message = updateContext.Update.CallbackQuery != null
                ? updateContext.Update.CallbackQuery.Message
                : updateContext.Update.Message;

            EditedMessage = updateContext.Update.EditedMessage;
            MessageOrEdited = updateContext.Update.Message
                              ?? updateContext.Update.EditedMessage
                              ?? updateContext.Update.CallbackQuery.Message;

            if (Message != null)
            {
                TimeInit = Message.Date.GetDelay();
            }
        }

        public IUpdateContext Context { get; set; }
        private string AppendText { get; set; }
        public ITelegramBotClient Client { get; set; }
        public Message Message { get; set; }
        public Message EditedMessage { get; set; }
        public Message MessageOrEdited { get; set; }
        public int SentMessageId { get; internal set; }
        public int EditedMessageId { get; private set; }
        public int CallBackMessageId { get; set; }
        private string TimeInit { get; set; }
        private string TimeProc { get; set; }

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

        public async Task LeaveChat(long chatId = 0)
        {
            try
            {
                var chatTarget = chatId;
                if (chatId == 0)
                {
                    chatTarget = Message.Chat.Id;
                }

                Log.Information($"Leaving from {chatTarget}");
                await Client.LeaveChatAsync(chatTarget);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error LeaveChat.");
            }
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
                Log.Information($"Sending message to {chatTarget}");
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
                Log.Error(apiRequestException, $"SendMessage Ex1");

                try
                {
                    Log.Information($"Try Sending message to {chatTarget} without reply to Msg Id.");
                    send = await Client.SendTextMessageAsync(
                        chatTarget,
                        sendText,
                        ParseMode.Html,
                        replyMarkup: replyMarkup
                    );
                }
                catch (ApiRequestException apiRequestException2)
                {
                    Log.Error(apiRequestException2, $"SendMessage Ex2");
                }
            }

            if (send != null) SentMessageId = send.MessageId;
        }

        public async Task SendMediaAsync(string fileId, MediaType mediaType, string caption = "",
            IReplyMarkup replyMarkup = null, int replyToMsgId = -1)
        {
            Log.Information($"Sending media: {mediaType}, fileId: {fileId} to {Message.Chat.Id}");
            switch (mediaType)
            {
                case MediaType.Document:
                    await Client.SendDocumentAsync(Message.Chat.Id, fileId, caption, ParseMode.Html,
                        replyMarkup: replyMarkup, replyToMessageId: replyToMsgId);
                    break;

                case MediaType.LocalDocument:
                    var fileName = Path.GetFileName(fileId);
                    await using (var fs = File.OpenRead(fileId))
                    {
                        InputOnlineFile inputOnlineFile = new InputOnlineFile(fs, fileName);
                        await Client.SendDocumentAsync(Message.Chat.Id, inputOnlineFile, caption, ParseMode.Html,
                            replyMarkup: replyMarkup, replyToMessageId: replyToMsgId);
                    }

                    break;

                case MediaType.Photo:
                    await Client.SendPhotoAsync(Message.Chat.Id, fileId, caption, ParseMode.Html,
                        replyMarkup: replyMarkup, replyToMessageId: replyToMsgId);
                    break;
                
                case MediaType.Video:
                    await Client.SendVideoAsync(Message.Chat.Id, fileId, caption: caption, parseMode: ParseMode.Html);
                    break;

                default:
                    Log.Information($"Media unknown: {mediaType}");
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
            try
            {
                Log.Information($"Editing {CallBackMessageId}");
                await Client.EditMessageTextAsync(
                    Message.Chat,
                    CallBackMessageId,
                    sendText,
                    ParseMode.Html,
                    replyMarkup: replyMarkup
                );
            }
            catch (Exception e)
            {
                Log.Error(e, "Error EditMessage");
            }
        }

        public async Task AppendTextAsync(string sendText, InlineKeyboardMarkup replyMarkup = null)
        {
            if (string.IsNullOrEmpty(AppendText))
            {
                Log.Information("Sending new message");
                AppendText = sendText;
                await SendTextAsync(AppendText, replyMarkup);
            }
            else
            {
                Log.Information("Next, edit existing message");
                AppendText += $"\n{sendText}";
                await EditAsync(AppendText, replyMarkup);
            }
        }

        public async Task DeleteAsync(int messageId = -1, int delay = 0)
        {
            Thread.Sleep(delay);

            try
            {
                var chatId = MessageOrEdited.Chat.Id;
                var msgId = messageId != -1 ? messageId : SentMessageId;

                Log.Information($"Delete MsgId: {msgId} on ChatId: {chatId}");
                await Client.DeleteMessageAsync(chatId, msgId);
            }
            catch (ChatNotFoundException chatNotFoundException)
            {
                Log.Error(chatNotFoundException, $"Error Delete NotFound");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error Delete Message");
            }
        }

        public void ResetTime()
        {
            Log.Information("Resetting time..");

            var msgDate = Message.Date;
            var currentDate = DateTime.UtcNow;
            msgDate = msgDate.AddSeconds(-currentDate.Second);
            msgDate = msgDate.AddMilliseconds(-currentDate.Millisecond);
            TimeInit = msgDate.GetDelay();
        }

        #endregion Message

        public async Task<string> DownloadFileAsync(string fileName)
        {
            var fileId = Message.GetFileId();
            if (fileId.IsNullOrEmpty()) fileId = Message.ReplyToMessage.GetFileId();
            Log.Information($"Downloading file {fileId}");


            var file = await Client.GetFileAsync(fileId);

            Log.Information($"DownloadFile: {file.ToJson(true)}");

            fileName = $"Storage/Cache/{fileName}".EnsureDirectory();
            using (var fileStream = File.OpenWrite(fileName))
            {
                await Client.DownloadFileAsync(filePath: file.FilePath, destination: fileStream);
                Log.Information($"File saved to {fileName}");
            }

            return fileName;
        }

        #region Member Exec

        public async Task<bool> KickMemberAsync(User user = null)
        {
            bool isKicked;
            var idTarget = user.Id;

            Log.Information($"Kick {idTarget} from {Message.Chat.Id}");
            try
            {
                await Client.KickChatMemberAsync(Message.Chat.Id, idTarget, DateTime.Now);
                isKicked = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Kick Member");
                isKicked = false;
            }

            return isKicked;
        }

        public async Task UnbanMemberAsync(User user = null)
        {
            var idTarget = user.Id;
            Log.Information($"Unban {idTarget} from {Message.Chat.Id}");
            try
            {
                await Client.UnbanChatMemberAsync(Message.Chat.Id, idTarget);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UnBan Member");
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
                Log.Error(apiRequestException, "Error Promote Member");
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

                Log.Error(apiRequestException, "Error Demote Member");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Demote Member Ex");
            }

            return requestResult;
        }

        #endregion
    }
}