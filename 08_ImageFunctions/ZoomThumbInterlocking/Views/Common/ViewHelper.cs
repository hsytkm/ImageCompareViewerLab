using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZoomThumb.Views.Common
{
    static class ViewHelper
    {

        public static T GetChildControl<T>(DependencyObject d) where T : DependencyObject
        {
            if (d is T control) return control;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                control = GetChildControl<T>(VisualTreeHelper.GetChild(d, i));
                if (control != default) return control;
            }
            return default;
        }

        public static Size GetControlActualSize(FrameworkElement fe) =>
            (fe is null) ? default : new Size(fe.ActualWidth, fe.ActualHeight);

        public static Size GetImageSourcePixelSize(Image image)
        {
            if (!(image?.Source is BitmapSource source)) return default;
            return new Size(source.PixelWidth, source.PixelHeight);
        }

    }
}
