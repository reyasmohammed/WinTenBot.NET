using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public static JArray ToArray(this string data)
        {
            return JArray.Parse(data);
        }

        public static List<string> SplitText(this string text, string delimiter)
        {
            return text.Split(delimiter).ToList();
        }
        
        public static string ResolveVariable(this string input, object parameters) {
            
            ConsoleHelper.WriteLine("Resolving variable..");
            var type = parameters.GetType();
            Regex regex = new Regex( "\\{(.*?)\\}" );
            var sb = new StringBuilder();
            var pos = 0;

            foreach (Match toReplace in regex.Matches( input )) {
                var capture = toReplace.Groups[ 0 ];
                var paramName = toReplace.Groups[ toReplace.Groups.Count - 1 ].Value;
                var property = type.GetProperty( paramName );
                if (property == null) continue;
                sb.Append( input.Substring( pos, capture.Index - pos) );
                sb.Append( property.GetValue( parameters, null ) );
                pos = capture.Index + capture.Length;
            }

            if (input.Length > pos + 1) sb.Append( input.Substring( pos ) );

            return sb.ToString();
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
        
        public static bool CheckUrlValid(this string source)
        {
            return Uri.TryCreate(source, UriKind.Absolute, out Uri uriResult) 
                   && uriResult.Scheme == Uri.UriSchemeHttps;
        }
    }
}