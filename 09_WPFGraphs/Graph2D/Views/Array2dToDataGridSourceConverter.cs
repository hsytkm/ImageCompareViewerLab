using System;
using System.Data;
using System.Globalization;
using System.Windows.Data;

namespace Graph2D.Views
{
    class Array2dToDataGridSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            ConvertSub<int>(value, targetType, parameter, culture);

        private static object ConvertSub<T>(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is T[,] array)) return null;

            var rows = array.GetLength(0);
            var columns = array.GetLength(1);
            if (rows == 0 || columns == 0) return null;

            var table = new DataTable();

            for (var c = 0; c < columns; c++)
                table.Columns.Add(new DataColumn($"C{c}"));

            for (var r = 0; r < rows; r++)
            {
                var newRow = table.NewRow();
                for (var c = 0; c < columns; c++)
                {
                    newRow[c] = array[r, c];
                }
                table.Rows.Add(newRow);
            }
            return table.DefaultView;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();

    }
}
