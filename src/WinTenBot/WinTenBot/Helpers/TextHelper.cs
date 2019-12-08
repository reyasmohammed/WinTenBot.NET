using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WinTenBot.Helpers
{
    public static class TextHelper
    {
        public static string ToJson(this object dataTable, bool indented = false)
        {
            return JsonConvert.SerializeObject(dataTable, indented ? Formatting.Indented : Formatting.None);
        }

        public static DataTable ToDataTable(this string data)
        {
            return JsonConvert.DeserializeObject<DataTable>(data);
        }

        public static List<string> SplitText(this string text, string delimiter)
        {
            return text.Split(delimiter).ToList();
        }

        public static async Task ToFile(this string content, string path)
        {
            ConsoleHelper.WriteLine($"Writing file to {path}");
            await File.WriteAllTextAsync(path, content);

//            var sw = new StreamWriter(path);
//            sw.Write(content);
//            sw.Close();
//            sw.Dispose();

//            using (var sw = new StreamWriter(@path))
//            {
//                await sw.WriteAsync(content);
//                sw.Close();
//                sw.Dispose();
//            }

//            var buffer = Encoding.UTF8.GetBytes(content);
//
//            using (var fs = new FileStream(@path, FileMode.OpenOrCreate, 
//                FileAccess.Write, FileShare.None, buffer.Length, true))
//            {
//                await fs.WriteAsync(buffer, 0, buffer.Length);
//                fs.Close();
//            }
        }

        public static string SqlEscape(this object str)
        {
            return Regex.Replace(str.ToString(), @"[\x00'""\b\n\r\t\cZ\\%_]",
                delegate(Match match)
                {
                    var v = match.Value;
                    switch (v)
                    {
                        case "\x00": // ASCII NUL (0x00) character
                            return "\\0";

                        case "\b": // BACKSPACE character
                            return "\\b";

                        case "\n": // NEWLINE (linefeed) character
                            return "\\n";

                        case "\r": // CARRIAGE RETURN character
                            return "\\r";

                        case "\t": // TAB
                            return "\\t";

                        case "\u001A": // Ctrl-Z
                            return "\\Z";

                        default:
                            return "\\" + v;
                    }
                });
        }
    }
}