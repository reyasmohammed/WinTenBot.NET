using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using WinTenBot.Helpers;
using WinTenBot.Providers;

namespace WinTenBot.Handlers.Commands.Security
{
    public class WordSyncCommand : CommandBase
    {
        private RequestProvider _requestProvider;

        public override async Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args,
            CancellationToken cancellationToken)
        {
            _requestProvider = new RequestProvider(context);

            var isSudoer = _requestProvider.IsSudoer();
            var isAdmin = await _requestProvider.IsAdminGroup();

            if (isSudoer)
            {
                await _requestProvider.DeleteAsync(_requestProvider.Message.MessageId);
                
                await _requestProvider.AppendTextAsync("Sedang mengsinkronkan Word Filter");
                await DataHelper.SyncWordToLocalAsync();
                await _requestProvider.AppendTextAsync("Selesai mengsinkronkan.");
                
                await _requestProvider.DeleteAsync(delay:3000);

            }
        }
    }
}