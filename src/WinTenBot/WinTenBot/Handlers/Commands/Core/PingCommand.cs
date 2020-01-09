using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Model;

namespace WinTenBot.Handlers.Commands.Core
{
    internal class PingCommand : CommandBase
    {
        // private ChatProcessor _chatProcessor;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            // _chatProcessor = new ChatProcessor(context);
            ChatHelper.Init(context);

            var msg = context.Update.Message;

            var keyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData("Ping", "PONG")
            );

            await "ℹ️ Pong!!".AppendTextAsync();
            var isSudoer = msg.From.Id.IsSudoer();

            if (msg.Chat.Type == ChatType.Private && isSudoer)
            {
                // await "\n<b>Engine info.</b>".AppendTextAsync();
                await "🎛 <b>Engine info.</b>".AppendTextAsync();

                // var getWebHookInfo = await _chatProcessor.Client.GetWebhookInfoAsync(cancellationToken);
                var getWebHookInfo = await Bot.Client.GetWebhookInfoAsync(cancellationToken);
                if (getWebHookInfo.Url == "")
                {
                    // sendText += "\n\n<i>Bot run in Poll mode.</i>";
                    await "\n<i>Bot run in Poll mode.</i>".AppendTextAsync(keyboard);
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
                    
                    await hookInfo.AppendTextAsync(keyboard);
                }
            }

            // await sendText.AppendTextAsync(keyboard);

            ChatHelper.Close();
        }
    }
}