using System;

namespace WinTenBot.Helpers
{
    public static class TimeHelper
    {
        public static string GetDelay(this DateTime time)
        {
            var date1 = DateTime.Now.ToUniversalTime();
            var date2 = time;
            // Console.WriteLine($"Date1: {date1}, Date2: {date2}");

            var timeSpan = (date1 - date2);

            return timeSpan.ToString(@"s\,fff");
        }
    }
}