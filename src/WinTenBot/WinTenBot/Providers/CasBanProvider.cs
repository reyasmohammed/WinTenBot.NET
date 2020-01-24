using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Serilog;
using WinTenBot.Helpers;

namespace WinTenBot.Providers
{
    public class CasBanProvider
    {
        public async Task<dynamic> IsCasBan(int userId)
        {
            bool isBan = false;
            var url = "https://api.cas.chat/check".SetQueryParam("user_id", userId);
            var resp = await url.GetJsonAsync();
            Log.Debug("CasBan Response",resp);
            
            isBan = resp.ok;
            ConsoleHelper.WriteLine($"UserId: {userId} is CAS ban: {isBan}");
            return isBan;
            
//            if (resp.ok == true)
//            {
//                ConsoleHelper.WriteLine("CAS");
//            }
//            else
//            {
//                ConsoleHelper.WriteLine("No");
//            }
//            
//            Console.WriteLine(resp);
//            return resp;
        }
    }
}