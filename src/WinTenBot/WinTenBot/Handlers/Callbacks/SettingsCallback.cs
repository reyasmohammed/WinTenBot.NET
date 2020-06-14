﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Types;
using WinTenBot.Common;
using WinTenBot.Services;
using WinTenBot.Telegram;

namespace WinTenBot.Handlers.Callbacks
{
    public class SettingsCallback
    {
        private TelegramService _telegramService;
        private CallbackQuery CallbackQuery { get; set; }
        private Message Message { get; set; }

        public SettingsCallback(TelegramService telegramService)
        {
            _telegramService = telegramService;
            Message = telegramService.Message;
            CallbackQuery = telegramService.Context.Update.CallbackQuery;

            Log.Information("Receiving Setting Callback.");

            Parallel.Invoke(async () => await ExecuteToggleAsync());
        }

        private async Task ExecuteToggleAsync()
        {
            var chatId = CallbackQuery.Message.Chat.Id;
            var fromId = CallbackQuery.From.Id;
            var msgId = CallbackQuery.Message.MessageId;

            var isAdmin = await _telegramService.IsAdminGroup(fromId);
            if (!isAdmin)
            {
                Log.Information("He is not admin.");
                return;
            }
            
            var callbackData = CallbackQuery.Data;
            var partedData = callbackData.Split(" ");
            var callbackParam = partedData.ValueOfIndex(1);
            var partedParam = callbackParam.Split("_");
            var valueParamStr = partedParam.ValueOfIndex(0);
            var keyParamStr = callbackParam.Replace(valueParamStr, "");
            var currentVal = valueParamStr.ToBoolInt();

            Log.Information($"Param : {keyParamStr}");
            Log.Information($"CurrentVal : {currentVal}");

            var columnTarget = "enable" + keyParamStr;
            var newValue = currentVal == 0 ? 1 : 0;
            Log.Information($"Column: {columnTarget}, Value: {currentVal}, NewValue: {newValue}");

            var settingService = new SettingsService(Message);
            var data = new Dictionary<string, object>()
            {
                ["chat_id"] = chatId,
                [columnTarget] = newValue
            };

            await settingService.SaveSettingsAsync(data);

            var settingBtn = await settingService.GetSettingButtonByGroup();
            var btnMarkup = await settingBtn.ToJson().JsonToButton(chunk: 2);
            Log.Debug($"Settings: {settingBtn.Count}");

            _telegramService.SentMessageId = msgId;
            
            var editText = $"Settings Toggles" +
                           $"\nParam: {columnTarget} to {newValue}";
            await _telegramService.EditMessageCallback(editText, btnMarkup);
            
            // var lastReplyMarkup = CallbackQuery.Message.ReplyMarkup.InlineKeyboard;
            // Log.Debug($"LastReplyMarkup: {lastReplyMarkup.ToJson(true)}");
        }
    }
}