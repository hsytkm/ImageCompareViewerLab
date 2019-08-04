using System;
using System.Globalization;
using System.Windows.Data;

namespace ImageMetaExtractorApp.Views
{
    [ValueConversion(typeof(bool), typeof(int))]
    class BooleanToIntParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool b)) return 0;
            if (!(parameter is int size)) return 100;
            return b ? size : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
