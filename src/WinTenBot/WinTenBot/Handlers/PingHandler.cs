using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;

namespace WinTenBot.Handlers
{
    internal class PingHandler : IUpdateHandler
    {
        // private ChatProcessor _chatProcessor;

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            // _chatProcessor = new ChatProcessor(context);
            ChatHelper.Init(context);
            var msg = context.Update.Message;
            
            var keyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData("Ping", "PONG")
            );
            
            var sendText = "ℹ️ Pong!!";
            var isSudoer = msg.From.Id.IsSudoer();
            
            if (msg.Chat.Type == ChatType.Private && isSudoer)
            {
                sendText += "\n🎛 <b>Engine info.</b>";
                var getWebHookInfo = await ChatHelper.Client.GetWebhookInfoAsync(cancellationToken);
                if (getWebHookInfo.Url == "")
                {
                    sendText += "\n\n<i>Bot run in Poll mode.</i>";
                }
                else
                {
                    sendText += "\n\n<i>Bot run in Webhook mode.</i>" +
                                $"\nUrl WebHook: {getWebHookInfo.Url}" +
                                $"\nUrl Custom Cert: {getWebHookInfo.HasCustomCertificate}" +
                                $"\nAllowed Updates: {getWebHookInfo.AllowedUpdates}" +
                                $"\nPending Count: {getWebHookInfo.PendingUpdateCount}" +
                                $"\nMaxConnection: {getWebHookInfo.MaxConnections}" +
                                $"\nLast Error: {getWebHookInfo.LastErrorDate}" +
                                $"\nError Message: {getWebHookInfo.LastErrorMessage}";
                }
            }
            
            await sendText.SendTextAsync(keyboard);
            
            ChatHelper.Close();
        }
    }
}