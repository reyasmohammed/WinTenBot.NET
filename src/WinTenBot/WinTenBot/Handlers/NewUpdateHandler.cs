using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Model;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers
{
    public class NewUpdateHandler : IUpdateHandler
    {
        private readonly AfkService _afkService;

        public NewUpdateHandler()
        {
            _afkService = new AfkService();
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            ChatHelper.Init(context);

            ConsoleHelper.WriteLine("New Update");
            if (EnvironmentHelper.IsDev())
            {
                context.ToJson().ToConsoleStamp();
            }

            var message = context.Update.Message ?? context.Update.CallbackQuery.Message;

            await AfkCheck(message);
            await CheckUsername(message);

            if (!ChatHelper.IsPrivateChat())
            {
                await CheckGlobalBanAsync(message);
            }

#pragma warning disable 4014
            HitActivity(message);
#pragma warning restore 4014

            ChatHelper.Close();

            await next(context, cancellationToken);
        }

        private async Task AfkCheck(Message message)
        {
            if (message.ReplyToMessage != null)
            {
                var repMsg = message.ReplyToMessage;
                var isAfkReply = await _afkService.IsAfkAsync(repMsg);
                if (isAfkReply)
                    await $"{repMsg.GetFromNameLink()} sedang afk".SendTextAsync();
            }

            var isAfk = await _afkService.IsAfkAsync(message);
            if (isAfk)
            {
                await $"{message.GetFromNameLink()} sudah tidak afk".SendTextAsync();

                var data = new Dictionary<string, object>()
                {
                    {"chat_id", message.Chat.Id},
                    {"user_id", message.From.Id},
                    {"is_afk", 0},
                    {"afk_reason", ""}
                };

                await _afkService.SaveAsync(data);
                await _afkService.UpdateCacheAsync();
            }
        }

        private async Task CheckGlobalBanAsync(Message message)
        {
            var userId = message.From.Id;
            var user = message.From;
            var messageId = message.MessageId;

            // var isBan = await _elasticSecurityService.IsExist(userId);
            var isBan = await user.IsBanInCache();
            ConsoleHelper.WriteLine($"IsBan: {isBan}");
            if (isBan)
            {
                await ChatHelper.DeleteAsync(messageId);
                await ChatHelper.KickMemberAsync(user);
                await ChatHelper.UnbanMemberAsync(user);
            }
        }

        private async Task CheckUsername(Message message)
        {
            var fromUser = message.From;
            var noUsername = fromUser.IsNoUsername();
            ConsoleHelper.WriteLine($"{fromUser} IsNoUsername: {noUsername}");

            if (noUsername)
            {
                await $"{fromUser} belum memasang username".SendTextAsync();
            }
        }

        private async Task HitActivity(Message message)
        {
            var data = new Dictionary<string,object>()
            {
                {"via_bot","ZiziBeta"},
                {"message_type",message.Type.ToString()},
                {"from_id",message.From.Id},
                {"from_first_name",message.From.FirstName},
                {"from_last_name",message.From.LastName},
                {"from_username",message.From.Username},
                {"from_lang_code",message.From.LanguageCode},
                {"chat_id",message.Chat.Id},
                {"chat_username",message.Chat.Username},
                {"chat_type",message.Chat.Type.ToString()},
                {"chat_title",message.Chat.Title},
            };
            
            var insertHit = await new Query("hit_activity")
                .ExecForMysql()
                .InsertAsync(data);
            ConsoleHelper.WriteLine($"Insert Hit: {insertHit}");
        }
    }
}