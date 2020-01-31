using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Flurl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using WinTenBot.Model;
using WinTenBot.Providers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WinTenBot.Helpers
{
    public static class DataHelper
    {
        public static IEnumerable<DataRow> AsEnumerableX(this DataTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                yield return table.Rows[i];
            }
        }

        public static string GenerateUrlQrApi(this string data)
        {
            return $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&margin=10&data={Url.Encode(data)}";
        }

        public static void SaveUrlTo(this string remoteFileUrl, string localFileName)
        {
            var webClient = new WebClient();

            ConsoleHelper.WriteLine($"Saving {remoteFileUrl} to {localFileName}");
            webClient.DownloadFile(remoteFileUrl, localFileName);
            webClient.Dispose();
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> ts) where T : class
        {
            var dt = new DataTable();
            //Get Enumerable Type
            Type tT = typeof(T);

            //Get Collection of NoVirtual properties
            var props = tT.GetProperties().Where(p => !p.GetGetMethod().IsVirtual).ToArray();

            //Fill Schema
            foreach (PropertyInfo p in props)
                dt.Columns.Add(p.Name, p.GetMethod.ReturnParameter.ParameterType.BaseType);

            //Fill Data
            foreach (T t in ts)
            {
                DataRow row = dt.NewRow();

                foreach (PropertyInfo p in props)
                    row[p.Name] = p.GetValue(t);

                dt.Rows.Add(row);
            }

            return dt;
        }

        public static T MapObject<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        // Source: https://www.codeproject.com/Articles/44274/Transpose-a-DataTable-using-C
        public static DataTable TransposedTable(this DataTable inputTable)
        {
            DataTable outputTable = new DataTable();

            // Add columns by looping rows

            // Header row's first column is same as in inputTable
            outputTable.Columns.Add(inputTable.Columns[0].ColumnName.ToString());

            // Header row's second column onwards, 'inputTable's first column taken
            foreach (DataRow inRow in inputTable.Rows)
            {
                string newColName = inRow[0].ToString();
                outputTable.Columns.Add(newColName);
            }

            // Add rows by looping columns        
            for (int rCount = 1; rCount <= inputTable.Columns.Count - 1; rCount++)
            {
                DataRow newRow = outputTable.NewRow();

                // First column is inputTable's Header row's second column
                newRow[0] = inputTable.Columns[rCount].ColumnName.ToString();
                for (int cCount = 0; cCount <= inputTable.Rows.Count - 1; cCount++)
                {
                    string colValue = inputTable.Rows[cCount][rCount].ToString();
                    newRow[cCount + 1] = colValue;
                }

                outputTable.Rows.Add(newRow);
            }

            return outputTable;
        }

        public static async Task SyncWordToLocalAsync()
        {
            var cloudQuery = await new Query("word_filter")
                .ExecForMysql()
                .GetAsync();

            var cloudWords = cloudQuery.ToJson().MapObject<List<WordFilter>>();

            var clearData = await new Query("word_filter")
                .ExecForSqLite(true)
                .DeleteAsync();

            Log.Information($"Deleting local Word Filter: {clearData} rows");

            foreach (var row in cloudWords)
            {
                var data = new Dictionary<string, object>()
                {
                    {"word", row.Word},
                    {"is_global", row.IsGlobal},
                    {"deep_filter", row.DeepFilter},
                    {"from_id", row.FromId},
                    {"chat_id", row.ChatId},
                    {"created_at", row.CreatedAt}
                };

                var insert = await new Query("word_filter")
                    .ExecForSqLite(true)
                    .InsertAsync(data);
            }
        }
    }
}