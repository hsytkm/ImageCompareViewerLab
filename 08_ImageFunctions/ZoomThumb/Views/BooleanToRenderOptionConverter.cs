using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ZoomThumb.Views
{
    /// <summary>
    /// フラグをRenderOptionに変換
    /// </summary>
    class BooleanToRenderOptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // true=高解像度で画素見える / false=フィルタ
            if (value is bool b && b) return BitmapScalingMode.HighQuality;
            return BitmapScalingMode.NearestNeighbor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

}
