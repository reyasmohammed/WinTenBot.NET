using System.Threading.Tasks;
using Serilog;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Callbacks
{
    public class HelpCallback
    {
        private string CallBackData { get; set; }
        private TelegramProvider _telegramProvider;
        
        public HelpCallback(TelegramProvider telegramProvider)
        {
            _telegramProvider = telegramProvider;
            CallBackData = telegramProvider.CallbackQuery.Data;
            
            Parallel.Invoke(async ()=> await ExecuteAsync());
        }

        private async Task ExecuteAsync()
        {
            var partsCallback = CallBackData.SplitText(" ");
            var sendText = await partsCallback[1].LoadInBotDocs();
            Log.Information($"Docs: {sendText}");
            var subPartsCallback = partsCallback[1].SplitText("/");

            Log.Information($"SubParts: {subPartsCallback.ToJson()}");
            var jsonButton = partsCallback[1];

            if (subPartsCallback.Count > 1)
            {
                jsonButton = subPartsCallback[0];

                switch (subPartsCallback[1])
                {
                    case "info":
                        jsonButton = subPartsCallback[1];
                        break;
                }
            }

            var keyboard = await $"Storage/Buttons/{jsonButton}.json".JsonToButton();


            await _telegramProvider.EditMessageCallback(sendText, keyboard);
        }
        
        
    }
}