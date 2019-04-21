using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Graph2D.Views
{
    class AttachProperties
    {
        public static object GetBackgroundEvent(DependencyObject obj) =>
            obj.GetValue(BackgroundEventProperty);

        public static void SetBackgroundEvent(DependencyObject obj, object value) =>
            obj.SetValue(BackgroundEventProperty, value);

        private static readonly string BackgroundEvent = nameof(BackgroundEvent);
        public static readonly DependencyProperty BackgroundEventProperty =
            DependencyProperty.RegisterAttached(
                BackgroundEvent,
                typeof(object),
                typeof(AttachProperties),
                new PropertyMetadata(
                    null,
                    (sender, e) =>
                    {
                        if (!(sender is Border border)) return;
                        if (!(e.NewValue is TextBlock text)) return;

                        text.Loaded += ((_, __) =>
                        {
                            try
                            {
                                var lv1 = byte.Parse(text.Text);
                                var lv2 = (byte)(255 - lv1);

                                var bg = new SolidColorBrush(Color.FromArgb(0xFF, lv1, lv1, 0x00));
                                var fg = new SolidColorBrush(Color.FromArgb(0xFF, lv2, lv2, lv2));

                                border.Background = bg;
                                //text.Background = bg;
                                text.Foreground = fg;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        });
                    }));

    }
}
