using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers.Commands.Group
{
    public class ReportCommand : CommandBase
    {
        private ChatProcessor _chatProcessor;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            var msg = context.Update.Message;
            var sendText = "Balas pesan yg mau di report";

            if (msg.Chat.Type == ChatType.Private)
            {
                await _chatProcessor.SendAsync("Report hanya untuk grup saja");
                return;
            }

            if (msg.ReplyToMessage != null)
            {
                var repMsg = msg.ReplyToMessage;

                if (msg.From.Id != repMsg.From.Id)
                {
                    var mentionAdmins = await _chatProcessor.GetMentionAdminsStr();

                    sendText = $"Ada laporan nich." +
                               $"\n{msg.GetFromNameLink()} melaporkan {repMsg.GetFromNameLink()}" +
                               $"{mentionAdmins}";

                    var keyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Hapus", "PONG"),
                            InlineKeyboardButton.WithCallbackData("Tendang", "PONG"),
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Ban", "PONG"),
                            InlineKeyboardButton.WithCallbackData("Ke Pesan", "PONG"),
                        }
                    });
                    
                    await _chatProcessor.SendAsync(sendText);
                    return;
                }

                sendText = "Melaporkan diri sendiri? 🤔";
            }


            await _chatProcessor.SendAsync(sendText);
        }
    }
}