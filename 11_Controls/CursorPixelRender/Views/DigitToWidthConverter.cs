using System;
using System.Globalization;
using System.Windows.Data;

namespace CursorPixelRender.Views
{
    [ValueConversion(typeof(int), typeof(double))]
    class DigitToWidthConverter : IValueConverter
    {
        /// <summary>
        /// 数値の桁数からViewの大体の幅を求める(FontSizeとか考慮してない)
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int charWidth = 10;
            if (value is int digit) return digit * charWidth;

            throw new ArgumentException(nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
