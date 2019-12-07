using System.Collections.Generic;
using System.Data;

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
    }
}