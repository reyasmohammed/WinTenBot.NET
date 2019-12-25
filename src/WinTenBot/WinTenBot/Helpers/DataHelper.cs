using System.Collections.Generic;
using System.Data;
using Flurl;

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
    }
}