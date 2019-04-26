using Graph2D.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Graph2D.Views
{
    class ColoredObjectBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ColoredObject[,] data)
            {
                return ConvertColoredObjectRows(data);
            }
            return null;
        }

        private static IReadOnlyList<ColoredObjectRow> ConvertColoredObjectRows(ColoredObject[,] source)
        {
            var rowLength = source.GetLength(0);
            if (rowLength == 0) throw new ArgumentException(nameof(rowLength));

            var columnLength = source.GetLength(1);
            if (columnLength == 0) throw new ArgumentException(nameof(columnLength));

            var rows = new List<ColoredObjectRow>(rowLength);
            for (var r = 0; r < rows.Capacity; r++)
            {
                rows.Add(new ColoredObjectRow(r, source));
            }
            return rows;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

}
