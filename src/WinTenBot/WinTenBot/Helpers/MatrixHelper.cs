namespace WinTenBot.Helpers
{
    public static class MatrixHelper
    {
        public static T[,] TransposeMatrix<T>(this T[,] matrix)
        {
            var rows    = matrix.GetLength(0);
            var columns = matrix.GetLength(1);

            var result = new T[columns, rows];

            for (var c = 0; c < columns; c++)
            {
                for (var r = 0; r < rows; r++)
                {
                    result[c, r] = matrix[r, c];
                }
            }

            return result;
        }
    }
}