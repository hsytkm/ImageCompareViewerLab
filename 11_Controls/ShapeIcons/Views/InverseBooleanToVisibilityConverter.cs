using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShapeIcons.Views
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b) return Visibility.Hidden;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();

    }
}
