using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace WinTenBot.Helpers
{
    public static class ArrayHelper
    {
        public static T[][] ChunkBy<T>(this IEnumerable<T> btnList, int chunk = 2)
        {
            Log.Information($"Chunk buttons to {chunk}");
            var chunksBtn = btnList
                .Select((s, i) => new { Value = s, Index = i })
                .GroupBy(x => x.Index / chunk)
                .Select(grp => grp.Select(x => x.Value).ToArray())
                .ToArray();
            
            return chunksBtn;
        }

        public static bool IsNullIndex(this object[] array, int index)
        {
            if (array.Length <= index) return false;
            
            return array[index] != null;
        }
        
        public static string ValueOfIndex(this string[] array, int index)
        {
            string value = string.Empty;
            if (array.Length > index && array[index] != null)
            {
                value = array[index];
                Log.Debug($"Get Array index {index}: {value}");
            }

            return value;
        }
    }
}