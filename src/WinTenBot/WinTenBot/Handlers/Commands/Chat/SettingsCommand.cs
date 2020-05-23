using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Chat
{
    public class SettingsCommand : CommandBase
    {
        private TelegramService _telegramService;
        private SettingsService _settingsService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            _settingsService = new SettingsService(context.Update.Message);
            var message = _telegramService.Message;

            await _telegramService.DeleteAsync(message.MessageId);

            var adminOrPrivate = await _telegramService.IsAdminOrPrivateChat();
            if (adminOrPrivate)
            {
                await _telegramService.SendTextAsync("Sedang mengambil pengaturan..");
                var settings = await _settingsService.GetSettingButtonByGroup();

                var btnMarkup = await settings.ToJson().JsonToButton(chunk: 2);
                Log.Debug($"Settings: {settings.Count}");

                await _telegramService.EditAsync("Settings Toggles", btnMarkup);
            }
        }
    }
}