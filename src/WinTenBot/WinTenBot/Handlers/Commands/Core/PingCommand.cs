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
        private TelegramProvider _telegramProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);

            var msg = _telegramProvider.MessageOrEdited;

            var keyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData("Ping", "PONG")
            );

            await _telegramProvider.AppendTextAsync("ℹ️ Pong!!");
            var isSudoer = msg.From.Id.IsSudoer();

            if (msg.Chat.Type == ChatType.Private && isSudoer)
            {
                // await "\n<b>Engine info.</b>".AppendTextAsync();
                await _telegramProvider.AppendTextAsync("🎛 <b>Engine info.</b>");

                // var getWebHookInfo = await _chatProcessor.Client.GetWebhookInfoAsync(cancellationToken);
                var getWebHookInfo = await _telegramProvider.Client.GetWebhookInfoAsync(cancellationToken);
                if (getWebHookInfo.Url == "")
                {
                    // sendText += "\n\n<i>Bot run in Poll mode.</i>";
                    await _telegramProvider.AppendTextAsync("\n<i>Bot run in Poll mode.</i>", keyboard);
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

                    await _telegramProvider.AppendTextAsync(hookInfo, keyboard);
                }
            }

            // await sendText.AppendTextAsync(keyboard);
        }
    }
}