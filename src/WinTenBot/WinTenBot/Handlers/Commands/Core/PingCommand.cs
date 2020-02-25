using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Core
{
    internal class PingCommand : CommandBase
    {
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);

            var msg = context.Update.Message;

            var keyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData("Ping", "PONG")
            );

            await _requestProvider.AppendTextAsync("ℹ️ Pong!!");
            var isSudoer = msg.From.Id.IsSudoer();

            if (msg.Chat.Type == ChatType.Private && isSudoer)
            {
                // await "\n<b>Engine info.</b>".AppendTextAsync();
                await _requestProvider.AppendTextAsync("🎛 <b>Engine info.</b>");

                // var getWebHookInfo = await _chatProcessor.Client.GetWebhookInfoAsync(cancellationToken);
                var getWebHookInfo = await _requestProvider.Client.GetWebhookInfoAsync(cancellationToken);
                if (getWebHookInfo.Url == "")
                {
                    // sendText += "\n\n<i>Bot run in Poll mode.</i>";
                    await _requestProvider.AppendTextAsync("\n<i>Bot run in Poll mode.</i>", keyboard);
                }
                else
                {
                    var hookInfo = "\n<i>Bot run in Webhook mode.</i>" +
                                   $"\nUrl WebHook: {getWebHookInfo.Url}" +
                                   $"\nUrl Custom Cert: {getWebHookInfo.HasCustomCertificate}" +
                                   $"\nAllowed Updates: {getWebHookInfo.AllowedUpdates}" +
                                   $"\nPending Count: {getWebHookInfo.PendingUpdateCount}" +
                                   $"\nMaxConnection: {getWebHookInfo.MaxConnections}" +
                                   $"\nLast Error: {getWebHookInfo.LastErrorDate}" +
                                   $"\nError Message: {getWebHookInfo.LastErrorMessage}";
                    
                    await _requestProvider.AppendTextAsync(hookInfo, keyboard);
                }
            }

            // await sendText.AppendTextAsync(keyboard);
        }
    }
}