using Telegram.Bot.Types;

namespace WinTenBot.Handlers.Callbacks
{
    public class HelpCallbackQuery
    {
        private string CallBackData { get; set; }
        public HelpCallbackQuery(CallbackQuery callbackQuery)
        {
            CallBackData = callbackQuery.Data;
        }
        
        
    }
}