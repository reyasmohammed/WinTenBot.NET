using System;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace WinTenBot.Helpers.Processors
{
    public class ChatProcessor
    {
        private IUpdateContext _updateContext;
        public long ChatId { get; private set; }
        public int SentMessageId { get; private set; }
        public int EditedMessageId { get; private set; }

        public ChatProcessor(IUpdateContext updateContext)
        {
            _updateContext = updateContext;

            ChatId = _updateContext.Update.Message.Chat.Id;
        }

        public async Task SendAsync(string sendText, IReplyMarkup replyMarkup = null, bool replyToReplied=false)
        {
            var replyToMsgId = _updateContext.Update.Message.MessageId;
            if (replyToReplied)
            {
                replyToMsgId = _updateContext.Update.Message.ReplyToMessage.MessageId;
            }
            var date = _updateContext.Update.Message.Date;
//            ConsoleHelper.WriteLine(date);
//            sendText += TimeHelper.Delay(_updateContext.Update.Message.Date);
            Message send = null;
            try
            {
                send = await _updateContext.Bot.Client.SendTextMessageAsync(
                    _updateContext.Update.Message.Chat,
                    sendText,
                    ParseMode.Html,
                    replyMarkup: replyMarkup,
                    replyToMessageId: replyToMsgId
                );
            }
            catch (ApiRequestException apiRequestException)
            {
                ConsoleHelper.WriteLine(apiRequestException.Message);

                try
                {
                    send = await _updateContext.Bot.Client.SendTextMessageAsync(
                        _updateContext.Update.Message.Chat,
                        sendText,
                        ParseMode.Html,
                        replyMarkup: replyMarkup
                    );
                }
                catch (ApiRequestException apiRequestException2)
                {
                    ConsoleHelper.WriteLine($"{apiRequestException2.ErrorCode}: {apiRequestException2.Message}");
                }
            }

            //            Console.WriteLine(TextHelper.ToJson(send));
            SentMessageId = send.MessageId;
        }

        public async Task EditAsync(string sendText)
        {
            Message edit = null;

            edit = await _updateContext.Bot.Client.EditMessageTextAsync(
                _updateContext.Update.Message.Chat,
                SentMessageId,
                sendText,
                ParseMode.Html
            );
                
            //            Console.WriteLine(TextHelper.ToJson(edit));
            EditedMessageId = edit.MessageId;
        }

        public async Task DeleteAsync(int messageId = -1)
        {
            var mssgId = messageId != -1 ? messageId : SentMessageId;

            try
            {
                ConsoleHelper.WriteLine($"Delete MsgId: {mssgId} on ChatId: {ChatId}");
                await _updateContext.Bot.Client.DeleteMessageAsync(ChatId, mssgId);
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
    }
}