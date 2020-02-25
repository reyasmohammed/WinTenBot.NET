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
        private RequestProvider _requestProvider;
        private WordFilterService _wordFilterService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            _wordFilterService = new WordFilterService(context.Update.Message);

            var msg = context.Update.Message;
            var cleanedMsg = msg.Text.GetTextWithoutCmd();
            var partedMsg = cleanedMsg.Split(" ");
            var paramOption = partedMsg.ValueOfIndex(1);
            var word = partedMsg.ValueOfIndex(0);
            var isGlobalBlock = false;

            var isSudoer = _requestProvider.IsSudoer();
            var isAdmin = await _requestProvider.IsAdminGroup();
            if (isSudoer || isAdmin)
            {
                var where = new Dictionary<string, object>() {{"word", word}};
                
                if (paramOption == "-g" && isSudoer)
                {
                    isGlobalBlock = true;
                    await _requestProvider.SendTextAsync("Kata ini akan di blokir dengan mode Group-wide!");
                }

                if (!isSudoer)
                {
                    await _requestProvider.SendTextAsync("Hanya Sudoer yang dapat memblokir Kata mode Group-wide!");
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

                        await _requestProvider.SendTextAsync(save.ToJson());
                    }
                    else
                    {
                        await _requestProvider.SendTextAsync("Sudah");
                    }
                }
                else
                {
                    await _requestProvider.SendTextAsync("Apa kata?");
                }
            }
        }
    }
}