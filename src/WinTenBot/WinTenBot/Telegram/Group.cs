﻿using System.Threading.Tasks;
using Serilog;
using WinTenBot.Common;
using WinTenBot.Services;

namespace WinTenBot.Telegram
{
    public static class Group
    {
        public static async Task SaveWelcome(this TelegramService telegramService, string target)
        {
            var msg = telegramService.Message;
            var chatId = msg.Chat.Id;
            var columnTarget = $"welcome_{target}";
            var data = msg.Text.GetTextWithoutCmd();
            var settingsService = new SettingsService(msg);

            if (data.IsNullOrEmpty())
            {
                await telegramService.SendTextAsync($"Silakan masukan konfigurasi {target} yang akan di terapkan")
                    .ConfigureAwait(false);
                return;
            }
            // Log.Information(columnTarget);
            // Log.Information(data);

            await telegramService.SendTextAsync($"Sedang menyimpan Welcome {target}..")
                .ConfigureAwait(false);

            await settingsService.UpdateCell(columnTarget, data)
                .ConfigureAwait(false);

            await telegramService.EditAsync($"Welcome {target} berhasil di simpan!" +
                                            $"\nKetik /welcome untuk melihat perubahan")
                .ConfigureAwait(false);

            Log.Information($"Success save welcome {target} on {chatId}.");
        }
    }
}