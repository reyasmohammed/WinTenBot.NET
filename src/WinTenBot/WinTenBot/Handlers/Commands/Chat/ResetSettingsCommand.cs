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
                    ["chat_title"] = chat.Title,
                    ["enable_afk_stats"] = 1,
                    ["enable_anti_malfiles"] = 1,
                    ["enable_fed_cas_ban"] = 1,
                    ["enable_fed_es2"] = 1,
                    ["enable_fed_spamwatch"] = 1,
                    ["enable_url_filtering"] = 1,
                    ["enable_human_verification"] = 1,
                    ["enable_reply_notification"] = 1,
                    ["enable_warn_username"] = 1,
                    ["enable_word_filter_group_wide"] = 1,
                    ["enable_word_filter_per_group"] = 1,
                    ["enable_welcome_message"] = 1,
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