using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinTenBot.Helpers
{
    public class MemberHelper
    {
        public static string GetNameLink(int userId, string Name)
        {
            return $"<a href='tg://user?id={userId}'>{Name}</a>";
        }
    }
}