using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Helpers.Processors;
using WinTenBot.Services;

namespace WinTenBot.Handlers.Commands.Tags
{
    public class TagCommand : CommandBase
    {
        private ChatProcessor _chatProcessor;
        private TagsService _tagsService;

        public TagCommand()
        {
            _tagsService = new TagsService();
        }

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            var msg = context.Update.Message;
            _chatProcessor = new ChatProcessor(context);
            var isSudoer = msg.From.Id.IsSudoer();

            var sendText = "ℹ Simpan tag ke Cloud Tags" +
                           "\nContoh: <code>/tag tagnya [tombol|link.tombol]</code> - Reply pesan" +
                           "\nPanjang tag minimal 3 karakter." +
                           "\nTanda [ ] artinya tidak harus" +
                           "\n" +
                           "\n📝 <i>Jika untuk grup, di rekomendasikan membuat sebuah channel, " +
                           "lalu link ke post di tautkan sebagai tombol.</i>";

            if (!isSudoer)
            {
                sendText = "This feature currently limited";
            }
            
            if (msg.ReplyToMessage != null && isSudoer)
            {
                ConsoleHelper.WriteLine("Replied message detected..");
                ConsoleHelper.WriteLine($"Arg0: {args[0]}");

                if (args[0].Length >= 3)
                {
                    await _chatProcessor.SendAsync("📖 Mengumpulkan informasi..");
                    ConsoleHelper.WriteLine(TextHelper.ToJson(msg.ReplyToMessage));
                    var content = EmojiHelper.ReplaceColonNames(msg.ReplyToMessage.Text);
                    ConsoleHelper.WriteLine(content);

                    var data = new Dictionary<string, object>()
                    {
                        {"id_chat", msg.Chat.Id},
                        {"id_user", msg.From.Id},
                        {"tag", args[0].Trim()},
                        {"content", content}
                    };

                    await _chatProcessor.EditAsync("📝 Menyimpan tag data..");
                    await _tagsService.SaveTag(data);
                    await _chatProcessor.EditAsync("✅ Tag berhasil di simpan..");
                }
            }
            else
            {
                foreach (var arg in args)
                {
                    Console.WriteLine(arg);
                }

                await _chatProcessor.SendAsync(sendText);
            }
        }
    }
}