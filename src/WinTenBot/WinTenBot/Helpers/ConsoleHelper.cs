using System;

namespace WinTenBot.Helpers
{
    public class ConsoleHelper
    {
        private static object BuildMsg(object message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            return $"{timestamp} - {message}";
        }
        public static void WriteLine(object message)
        {
            if (message == null) return;
            var forConsole = BuildMsg(message);
            Console.WriteLine(forConsole);
        }

        public static void Write(object message)
        {
            if (message == null) return;
            var forConsole = BuildMsg(message); 
            Console.WriteLine(forConsole);
        }
    }
}