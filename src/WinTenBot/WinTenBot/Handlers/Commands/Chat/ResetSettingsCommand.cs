using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Chat
{
    public class ResetSettingsCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;
        private SettingsService _settingsService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            _settingsService = new SettingsService(_telegramProvider.Message);
            var chat = _telegramProvider.Message.Chat;
            var msg = _telegramProvider.Message;

            var adminOrPrivate = await _telegramProvider.IsAdminOrPrivateChat();
            if (adminOrPrivate)
            {
                Log.Information("Initializing reset Settings.");
                await _telegramProvider.DeleteAsync(msg.MessageId);
                await _telegramProvider.SendTextAsync("Sedang mengembalikan ke Pengaturan awal");

                var data = new Dictionary<string, object>()
                {
                    ["chat_id"] = chat.Id,
                    ["enable_anti_malfiles"] = 1
                };
                
                var update = await _settingsService.SaveSettingsAsync(data);
                Log.Information($"Result: {update}");

                await _telegramProvider.EditAsync("Pengaturan awal berhasil di kembalikan");
                await _telegramProvider.DeleteAsync(_telegramProvider.EditedMessageId, 2000);
                Log.Information("Settings has been reset.");
            }
        }
    }
}