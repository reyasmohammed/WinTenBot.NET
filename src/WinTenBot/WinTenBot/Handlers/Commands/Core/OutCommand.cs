using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.LiteDB;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers.Commands.Core
{
    public class OutCommand:CommandBase
    {
        private ChatProcessor _chatProcessor;
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        { 
            _chatProcessor = new ChatProcessor(context);
            var msg = context.Update.Message;
            var partsMsg = msg.Text.GetTextWithoutCmd().Split("|").ToArray();

            var isSudoer = _chatProcessor.IsSudoer();
            if (isSudoer)
            {
                var sendText = "Maaf, saya harus keluar";

                if (partsMsg[1] != null)
                {
                    sendText += $"\n{partsMsg[1]}";
                }
                var chatId = partsMsg[0].ToInt64();

                ConsoleHelper.WriteLine($"Target out: {chatId}");
                await _chatProcessor.SendAsync(sendText, customChatId: chatId);
                await _chatProcessor.LeaveChat(chatId);
            }
            else
            {
                await _chatProcessor.SendAsync("Kamu tidak punya hak akses.");
            }
        }
    }
}