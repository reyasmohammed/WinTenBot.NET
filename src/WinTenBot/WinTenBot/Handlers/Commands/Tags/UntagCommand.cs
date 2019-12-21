using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Tags
{
    public class UntagCommand:CommandBase
    {
        private ChatProcessor _chatProcessor;
        private TagsService _tagsService;

        public UntagCommand()
        {
            _tagsService = new TagsService();
        }
        
        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            _chatProcessor = new ChatProcessor(context);
            var msg = context.Update.Message;

            var isAdmin = await _chatProcessor.IsAdminGroup();
            var tagVal = args[0];
            var sendText = "Perintah Untag hanya untuk ngadmin.";
            
            if (isAdmin)
            {
                await _chatProcessor.SendAsync("Memeriksa..");
                var isExist = await _tagsService.IsExist(_chatProcessor.Message.Chat.Id, tagVal);
                if (isExist)
                {
                    ConsoleHelper.WriteLine($"Sedang menghapus tag {tagVal}");
                    var unTag = await _tagsService.DeleteTag(_chatProcessor.Message.Chat.Id, tagVal);
                    if (unTag)
                    {
                        sendText = $"Hapus tag {tagVal} berhasil";
                    }

                    await _chatProcessor.EditAsync(sendText);
                    return;
                }
                else
                {
                    sendText = $"Tag {tagVal} tidak di temukan";
                }
            }
            
            await _chatProcessor.SendAsync(sendText);

        }
    }
}