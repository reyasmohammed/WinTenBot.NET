using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                ConsoleHelper.WriteLine($"Appending keyboard: {buttonLink[0]} -> {buttonLink[1]}");
                dict.Add(buttonLink[0], buttonLink[1]);
                
            }

            return dict;
        }
        public static InlineKeyboardMarkup CreateInlineKeyboardButton(Dictionary<string, string> buttonList, int columns)
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

        public static InlineKeyboardMarkup ToReplyMarkup(this string buttonStr, int columns)
        {
            return CreateInlineKeyboardButton(StringToDict(buttonStr), columns);
        }

        public static async Task<InlineKeyboardMarkup> JsonToButton(this string jsonPath, int chunk = 2)
        {
            var json = "";
            if (File.Exists(jsonPath))
            {
                Serilog.Log.Debug($"Loading Json from path: {jsonPath}");
                json = await File.ReadAllTextAsync(jsonPath);
            }
            else
            {
                Serilog.Log.Debug($"Loading Json from string..");
                json = jsonPath;
            }

            var replyMarkup = json.ToDataTable();
            
            var btnList = new List<InlineKeyboardButton>();

            foreach (DataRow row in replyMarkup.Rows)
            {
                var btnText = row["text"].ToString();
                var data = row["data"].ToString();
                if (data.CheckUrlValid())
                {
                    ConsoleHelper.WriteLine($"Appending Text: '{btnText}', Url: '{data}'.");
                    btnList.Add(InlineKeyboardButton.WithUrl(btnText, data));
                }
                else
                {
                    ConsoleHelper.WriteLine($"Appending Text: '{btnText}', Data: '{data}'.");
                    btnList.Add(InlineKeyboardButton.WithCallbackData(btnText,data));
                }
            }

            // ConsoleHelper.WriteLine($"Chunk buttons to {chunk}");
            // var chunksBtn = btnList
            //     .Select((s, i) => new { Value = s, Index = i })
            //     .GroupBy(x => x.Index / chunk)
            //     .Select(grp => grp.Select(x => x.Value).ToArray())
            //     .ToArray();
            
            return new InlineKeyboardMarkup(btnList.ChunkBy(chunk));
        }
    }
}