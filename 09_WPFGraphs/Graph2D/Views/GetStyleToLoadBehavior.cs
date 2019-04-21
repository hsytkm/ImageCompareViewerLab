using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Graph2D.Views
{
    class GetStyleToLoadBehavior
    {
        public static object GetStyleToLoad(DependencyObject obj) =>
            obj.GetValue(StyleToLoadProperty);

        public static void SetStyleToLoad(DependencyObject obj, object value) =>
            obj.SetValue(StyleToLoadProperty, value);

        public static readonly DependencyProperty StyleToLoadProperty =
            DependencyProperty.RegisterAttached(
                "StyleToLoad",
                typeof(object),
                typeof(GetStyleToLoadBehavior),
                new PropertyMetadata(
                    null,
                    (sender, e) =>
                    {
                        if (!(e.NewValue is TextBlock text)) return;
                        text.Loaded += ((sender2, _) =>
                        {
                            if (!(sender2 is TextBlock txt)) return;
                            var lv = byte.Parse(txt.Text);
                            var lv2 = (byte)(255 - lv);
                            text.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, lv, 0x00));
                            text.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, lv2, lv2, lv2));

                            if (!(sender is Border border)) return;
                            border.Background = text.Background;
                        });
                    }));

    }
}
