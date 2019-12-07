using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace WinTenBot.Helpers
{
    public static class KeyboardHelper
    {
        public static Dictionary<string,string> StringToDict(string buttonStr)
        {
            var dict = new Dictionary<string,string>();
            var splitWelcomeButton = buttonStr.Split(',').ToList<string>();
            foreach (var button in splitWelcomeButton)
            {
                var buttonLink = button.Split('|').ToList();
                Console.WriteLine($"Appending keyboard {buttonLink[0].ToString()} -> {buttonLink[1].ToString()}");
                dict.Add(buttonLink[0], buttonLink[1]);
                
            }

            return dict;
        }
        public static IReplyMarkup CreateInlineKeyboardButton(Dictionary<string, string> buttonList, int columns)
        {
            int rows = (int)Math.Ceiling((double)buttonList.Count / (double)columns);
            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[rows][];

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = buttonList
                    .Skip(i * columns)
                    .Take(columns)
                    .Select(direction => InlineKeyboardButton.WithUrl(direction.Key, direction.Value))
                    .ToArray();
            }
            return new InlineKeyboardMarkup(buttons);
        }

        public static IReplyMarkup ToReplyMarkup(this string buttonStr, int columns)
        {
            return CreateInlineKeyboardButton(StringToDict(buttonStr),columns);
        }
    }
}