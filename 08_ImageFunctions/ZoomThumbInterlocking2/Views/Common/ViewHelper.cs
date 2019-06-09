using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZoomThumb.Views.Common
{
    static class ViewHelper
    {
        public static T GetChildControl<T>(object obj) where T : DependencyObject
        {
            if (obj is null) return default;
            if (!(obj is DependencyObject d)) return default;
            return GetChildControl<T>(d);
        }

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

        public static bool IsValidValue(this double d)
        {
            if (double.IsNaN(d)) return false;
            if (d == 0.0) return false;
            return true;
        }

        public static bool IsValidValue(this Size size)
        {
            if (!size.Width.IsValidValue()) return false;
            if (!size.Height.IsValidValue()) return false;
            return true;
        }

        public static Rect Round(this Rect rect)
        {
            rect.X = Math.Round(rect.X);
            rect.Y = Math.Round(rect.Y);
            rect.Width = Math.Round(rect.Width);
            rect.Height = Math.Round(rect.Height);
            return rect;
        }
        
        public static Rect MinLength(this Rect rect, double minLength)
        {
            if (minLength <= 0.0) throw new ArgumentOutOfRangeException(nameof(minLength));

            if (rect.Width < minLength) rect.Width = minLength;
            if (rect.Height < minLength) rect.Height = minLength;
            return rect;
        }

    }
}
