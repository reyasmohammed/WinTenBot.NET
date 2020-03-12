using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using Telegram.Bot.Types;
using WinTenBot.Helpers;
using WinTenBot.Model;

namespace WinTenBot.Providers
{
    public static class AntiSpamProvider
    {
        public static async Task<SpamWatch> CheckSpamWatch(this int userId)
        {
            var spamWatch  = new SpamWatch();
            var spamWatchToken = Bot.GlobalConfiguration["CommonConfig:SpamWatchToken"];
            
            try
            {
                var baseUrl = $"https://api.spamwat.ch/banlist/{userId}";
                spamWatch = await baseUrl
                    .WithOAuthBearerToken(spamWatchToken)
                    .GetJsonAsync<SpamWatch>();
                spamWatch.IsBan = spamWatch.Code != 404;
                Log.Debug(spamWatch.ToJson(true));
            }
            catch (FlurlHttpException ex)
            {
                var callHttpStatus = ex.Call.HttpStatus;
                switch (callHttpStatus)
                {
                    case HttpStatusCode.NotFound:
                        spamWatch.IsBan = false;
                        break;
                    case HttpStatusCode.Unauthorized:
                        Log.Warning("Please check your SpamWatch API Token!");
                        break;
                }

                Log.Information($"StatusCode: {callHttpStatus}");
                Log.Error(ex, "SpamWatch API FlurlHttpEx");
            }
            catch (Exception ex)
            {
                Log.Error(ex,"SpamWatch Exception");
            }

            return spamWatch;
        }
        
        public static async Task<bool> CheckGBan(this int userId)
        {
            var query = await new Query("fban_user")
                .Where("user_id",userId)
                .ExecForSqLite(true)
                .GetAsync();
            
            var isGBan = query.Any();
            Log.Information($"UserId {userId} isGBan : {isGBan}");
            
            return isGBan;
        }
        
        public static async Task<bool> IsCasBanAsync(this User user)
        {
            var userId = user.Id;
            var url = "https://api.cas.chat/check".SetQueryParam("user_id", userId);
            var resp = await url.GetJsonAsync<CasBan>();
            
            Log.Debug("CasBan Response", resp);

            var isBan = resp.Ok;
            Log.Information($"UserId: {userId} is CAS ban: {isBan}");
            return isBan;

        }
    }
}