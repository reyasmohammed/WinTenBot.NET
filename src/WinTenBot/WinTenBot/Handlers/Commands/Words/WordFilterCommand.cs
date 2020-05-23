using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Words
{
    public class WordFilterCommand : CommandBase
    {
        private TelegramService _telegramService;
        private WordFilterService _wordFilterService;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _telegramService = new TelegramService(context);
            _wordFilterService = new WordFilterService(context.Update.Message);

            var msg = context.Update.Message;
            var cleanedMsg = msg.Text.GetTextWithoutCmd();
            var partedMsg = cleanedMsg.Split(" ");
            var paramOption = partedMsg.ValueOfIndex(1);
            var word = partedMsg.ValueOfIndex(0);
            var isGlobalBlock = false;

            var isSudoer = _telegramService.IsSudoer();
            var isAdmin = await _telegramService.IsAdminGroup();
            if (!isSudoer && !isAdmin)
            {
                return;
            }

            var where = new Dictionary<string, object>() { { "word", word } };

            if (paramOption.Contains("-"))
            {
                if (paramOption.Contains("g") && isSudoer) // Global
                {
                    isGlobalBlock = true;
                    await _telegramService.AppendTextAsync("Kata ini akan di blokir dengan mode Group-wide!");
                }

                if (paramOption.Contains("d"))
                {
                }

                if (paramOption.Contains("c"))
                {
                }
            }

            if (!paramOption.Contains("g"))
            {
                @where.Add("chat_id", msg.Chat.Id);
            }

            if (!isSudoer)
            {
                await _telegramService.AppendTextAsync("Hanya Sudoer yang dapat memblokir Kata mode Group-wide!");
            }

            if (word != "")
            {
                await _telegramService.AppendTextAsync("Sedang menambahkan kata");

                var isExist = await _wordFilterService.IsExistAsync(@where);
                if (!isExist)
                {
                    var save = await _wordFilterService.SaveWordAsync(word, isGlobalBlock);

                    await _telegramService.AppendTextAsync("Kata berhasil di tambahkan");
                }
                else
                {
                    await _telegramService.AppendTextAsync("Kata sudah di tambahkan");
                }
            }
            else
            {
                await _telegramService.SendTextAsync("Apa kata yg mau di blok?");
            }

            await _telegramService.DeleteAsync(delay: 3000);
        }
    }
}