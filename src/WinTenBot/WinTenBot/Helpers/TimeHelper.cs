using System;

namespace WinTenBot.Helpers
{
    public class TimeHelper
    {
        public static string Delay(DateTime time)
        {
            var date1 = DateTime.Now;
            var date2 = time;
            Console.WriteLine($"Date1: {date1}, Date2: {date2}");

            var delay = (date1 - date2).Milliseconds.ToString("hh:mm:ss:fff");
            Console.WriteLine($"Delay: {delay}");
            return delay.ToString();
        }
    }
}