using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Security
{
    public class WordFilterCommand : CommandBase
    {
        private TelegramProvider _telegramProvider;
        private WordFilterService _wordFilterService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramProvider = new TelegramProvider(context);
            _wordFilterService = new WordFilterService(context.Update.Message);

            var msg = context.Update.Message;
            var cleanedMsg = msg.Text.GetTextWithoutCmd();
            var partedMsg = cleanedMsg.Split(" ");
            var paramOption = partedMsg.ValueOfIndex(1);
            var word = partedMsg.ValueOfIndex(0);
            var isGlobalBlock = false;

            var isSudoer = _telegramProvider.IsSudoer();
            var isAdmin = await _telegramProvider.IsAdminGroup();
            if (isSudoer || isAdmin)
            {
                var where = new Dictionary<string, object>() {{"word", word}};

                if (paramOption == "-g" && isSudoer)
                {
                    isGlobalBlock = true;
                    await _telegramProvider.SendTextAsync("Kata ini akan di blokir dengan mode Group-wide!");
                }

                if (!isSudoer)
                {
                    await _telegramProvider.SendTextAsync("Hanya Sudoer yang dapat memblokir Kata mode Group-wide!");
                }

                if (paramOption != "-g")
                {
                    where.Add("chat_id", msg.Chat.Id);
                }

                if (word != "")
                {
                    var isExist = await _wordFilterService.IsExistAsync(where);
                    if (!isExist)
                    {
                        var save = await _wordFilterService.SaveWordAsync(word, isGlobalBlock);

                        await _telegramProvider.SendTextAsync(save.ToJson());
                    }
                    else
                    {
                        await _telegramProvider.SendTextAsync("Sudah");
                    }
                }
                else
                {
                    await _telegramProvider.SendTextAsync("Apa kata?");
                }
            }
        }
    }
}