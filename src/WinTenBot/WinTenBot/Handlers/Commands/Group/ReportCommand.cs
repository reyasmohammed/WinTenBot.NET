using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Group
{
    public class ReportCommand : CommandBase
    {
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;
            var sendText = "Balas pesan yg mau di report";

            if (msg.Chat.Type == ChatType.Private)
            {
                await _requestProvider.SendTextAsync("Report hanya untuk grup saja");
                return;
            }

            if (msg.ReplyToMessage != null)
            {
                var repMsg = msg.ReplyToMessage;

                if (msg.From.Id != repMsg.From.Id)
                {
                    var mentionAdmins = await _requestProvider.GetMentionAdminsStr();

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
                    
                    await _requestProvider.SendTextAsync(sendText);
                    return;
                }

                sendText = "Melaporkan diri sendiri? 🤔";
            }


            await _requestProvider.SendTextAsync(sendText);
        }
    }
}