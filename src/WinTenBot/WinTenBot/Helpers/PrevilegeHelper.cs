using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;
using WinTenBot.Model;

namespace WinTenBot.Helpers
{
    public static class PrevilegeHelper
    {
        public static bool IsSudoer(this int userId)
        {
            bool isSudoer = false;
            var sudoers = Bot.GlobalConfiguration.GetSection("Sudoers").Get<List<string>>();
//            foreach (var sudoer in sudoers)
//            {
//                if (sudoer != userId.ToString()) isSudoer = true;
//                break;
//            }
            var match = sudoers.FirstOrDefault(x => x == userId.ToString());
            if (match != null)
            {
                isSudoer = true;
            }
            Log.Information($"UserId: {userId} IsSudoer: {isSudoer}");
            return  isSudoer;
        }
    }
}