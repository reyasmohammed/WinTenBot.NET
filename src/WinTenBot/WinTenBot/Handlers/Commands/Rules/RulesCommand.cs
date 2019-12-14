﻿using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Rules
{
    public class RulesCommand : CommandBase
    {
        private ChatProcessor _chatProcessor;
        private SettingsService _settingsService;

        public RulesCommand()
        {
            _settingsService = new SettingsService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            var msg = context.Update.Message;
            _settingsService.ChatId = msg.Chat.Id;

            var sendText = "Under maintenance";
            if (msg.Chat.Type != ChatType.Private)
            {
                if (msg.From.Id.IsSudoer())
                {
                    var settings = await _settingsService.GetSettingByGroup(msg.Chat.Id);
                    await _settingsService.UpdateCache();
                    ConsoleHelper.WriteLine(settings.ToJson());
                    var rules = settings.Rows[0]["rules_text"].ToString();
                    ConsoleHelper.WriteLine(rules);
                    sendText = rules;
                }
            }
            else
            {
                sendText = "Rules hanya untuk grup";
            }

            await _chatProcessor.SendAsync(sendText);
        }
    }
}