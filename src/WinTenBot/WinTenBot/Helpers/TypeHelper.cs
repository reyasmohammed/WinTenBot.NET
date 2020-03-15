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
        
        public static bool ToBool(this string obj)
        {
            return Convert.ToBoolean(obj);
        }

        public static int ToInt(this object obj)
        {
            return Convert.ToInt32(obj);
        }
        
        public static long ToInt64(this object obj)
        {
            return Convert.ToInt64(obj);
        }

        public static int ToBoolInt(this string str)
        {
            return str.ToLower() == "disable" ? 0 : 1;
        }
    }
}