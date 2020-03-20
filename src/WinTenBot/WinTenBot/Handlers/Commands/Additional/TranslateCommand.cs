using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GoogleTranslateFreeApi;
using Serilog;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Additional
{
    public class TranslateCommand:CommandBase
    {
        private TelegramProvider _telegramProvider;
        
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        { 
            _telegramProvider = new TelegramProvider(context);

            var message = _telegramProvider.Message;

            if (message.ReplyToMessage != null)
            {
                var param = message.Text.SplitText(" ").ToArray();
                var param1 = param.ValueOfIndex(1) ?? "";

                // if (param1.IsNullOrEmpty() ||  !param1.Contains("-"))
                // {
                //     await _telegramProvider.SendTextAsync("Lang code di perlukan. Contoh: en-id, English ke Indonesia");
                //     return;
                // }

                if (param1.IsNullOrEmpty())
                {
                    param1 = message.From.LanguageCode;
                }
                
                var forTranslate = message.ReplyToMessage.Text;

                Log.Information($"Param: {param.ToJson(true)}");

                await _telegramProvider.SendTextAsync("🔄 Translating into Your language..");

                var translate = await forTranslate.Translate(param1);

                // var translate = forTranslate.TranslateTo(param1);
                
                // var translateResult = new StringBuilder();
                // foreach (var translation in translate.Result.Translations)
                // {
                    // translateResult.AppendLine(translation._Translation);
                // }

                var translateResult = translate.MergedTranslation;

                await _telegramProvider.EditAsync(translateResult);
            }
            else
            {
                await _telegramProvider.SendTextAsync("Reply message where you want to translate");
            }

        }
    }
}