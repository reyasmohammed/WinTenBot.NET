using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using Flurl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    }
}