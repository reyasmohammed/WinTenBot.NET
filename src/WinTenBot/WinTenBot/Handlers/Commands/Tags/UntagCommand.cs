using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Tags
{
    public class UntagCommand:CommandBase
    {
        private RequestProvider _requestProvider;
        private TagsService _tagsService;

        public UntagCommand()
        {
            _tagsService = new TagsService();
        }
        
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);
            var msg = context.Update.Message;

            var isAdmin = await _requestProvider.IsAdminGroup();
            var tagVal = args[0];
            var sendText = "Perintah Untag hanya untuk ngadmin.";
            
            if (isAdmin)
            {
                await _requestProvider.SendTextAsync("Memeriksa..");
                var isExist = await _tagsService.IsExist(_requestProvider.Message.Chat.Id, tagVal);
                if (isExist)
                {
                    ConsoleHelper.WriteLine($"Sedang menghapus tag {tagVal}");
                    var unTag = await _tagsService.DeleteTag(_requestProvider.Message.Chat.Id, tagVal);
                    if (unTag)
                    {
                        sendText = $"Hapus tag {tagVal} berhasil";
                    }

                    await _requestProvider.EditAsync(sendText);
                    return;
                }
                else
                {
                    sendText = $"Tag {tagVal} tidak di temukan";
                }
            }
            
            await _requestProvider.SendTextAsync(sendText);

        }
    }
}