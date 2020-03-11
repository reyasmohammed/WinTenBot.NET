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
        private TelegramProvider _telegramProvider;
        private SettingsService _settingsService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            _settingsService = new SettingsService(context.Update.Message);

            var adminOrPrivate = await _telegramProvider.IsAdminOrPrivateChat();
            if (adminOrPrivate)
            {
                await _telegramProvider.SendTextAsync("Sedang mengambil pengaturan..");
                var settings = await _settingsService.GetSettingButtonByGroup();

                var btnMarkup = await settings.ToJson().JsonToButton(chunk: 2);
                Log.Debug($"Settings: {settings.Count}");

                await _telegramProvider.EditAsync("Settings Toggles", btnMarkup);
            }
        }
    }
}