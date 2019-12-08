using System;

namespace WinTenBot.Helpers
{
    public static class TypeHelper
    {
        public static bool ToBool(this object obj)
        {
//            return bool.Parse(obj);
            return Convert.ToBoolean(obj);
        }
    }
}