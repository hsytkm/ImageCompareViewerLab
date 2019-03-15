// Two-Way Binding Of VerticalOffset Property on ScrollViewer?
// https://stackoverflow.com/questions/2096143/two-way-binding-of-verticaloffset-property-on-scrollviewer

using System;
using System.Windows;
using System.Windows.Controls;

namespace ZoomThumb.Views
{
    public class ScrollViewerBinding
    {
        #region VertHori

        /// <summary>
        /// ScrollOffset attached property
        /// </summary>
        public static readonly DependencyProperty ScrollOffsetProperty =
            DependencyProperty.RegisterAttached(
                "ScrollOffset",
                typeof(Size),
                typeof(ScrollViewerBinding),
                new FrameworkPropertyMetadata(
                    default(Size),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnScrollOffsetPropertyChanged));

        /// <summary>
        /// Just a flag that the binding has been applied.
        /// </summary>
        private static readonly DependencyProperty ScrollBindingProperty =
            DependencyProperty.RegisterAttached(
                "ScrollBinding",
                typeof(bool?),
                typeof(ScrollViewerBinding));

        public static Size GetScrollOffset(DependencyObject depObj) =>
            (Size)depObj.GetValue(ScrollOffsetProperty);

        public static void SetScrollOffset(DependencyObject depObj, Size value) =>
            depObj.SetValue(ScrollOffsetProperty, value);

        private static void OnScrollOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ScrollViewer scrollViewer)) return;

            if (scrollViewer.GetValue(ScrollBindingProperty) == null)
            {
                scrollViewer.SetValue(ScrollBindingProperty, true);
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }

            var size = (Size)e.NewValue;
            scrollViewer.ScrollToVerticalOffset(size.Height);
            scrollViewer.ScrollToHorizontalOffset(size.Width);
        }

        // ◆イベント解除できておらずメモリリークの一因な気がする…
        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(sender is ScrollViewer scrollViewer)) return;

            if (e.HorizontalChange != .0 || e.VerticalChange != .0)
            {
                SetScrollOffset(scrollViewer, new Size(e.HorizontalOffset, e.VerticalOffset));
            }
        }

        #endregion

        #region Vertical

        /// <summary>
        /// VerticalOffset attached property
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "VerticalOffset",
                typeof(double),
                typeof(ScrollViewerBinding),
                new FrameworkPropertyMetadata(
                    double.NaN,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnVerticalOffsetPropertyChanged));

        /// <summary>
        /// Just a flag that the binding has been applied.
        /// </summary>
        private static readonly DependencyProperty VerticalScrollBindingProperty =
            DependencyProperty.RegisterAttached(
                "VerticalScrollBinding",
                typeof(bool?),
                typeof(ScrollViewerBinding));

        public static double GetVerticalOffset(DependencyObject depObj) =>
            (double)depObj.GetValue(VerticalOffsetProperty);

        public static void SetVerticalOffset(DependencyObject depObj, double value) =>
            depObj.SetValue(VerticalOffsetProperty, value);

        private static void OnVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (double.IsNaN((double)e.NewValue)) return;
            if (!(d is ScrollViewer scrollViewer)) return;

            if (scrollViewer.GetValue(VerticalScrollBindingProperty) == null)
            {
                scrollViewer.SetValue(VerticalScrollBindingProperty, true);
                scrollViewer.ScrollChanged += ScrollViewer_VertivalScrollChanged;
            }
            scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        // ◆イベント解除できておらずメモリリークの一因な気がする…
        private static void ScrollViewer_VertivalScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(sender is ScrollViewer scrollViewer)) return;

            if (e.VerticalChange != .0)
            {
                SetVerticalOffset(scrollViewer, e.VerticalOffset);
            }
        }

        #endregion

        #region Horizonal

        /// <summary>
        /// HorizontalOffset attached property
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalOffset",
                typeof(double),
                typeof(ScrollViewerBinding),
                new FrameworkPropertyMetadata(
                    double.NaN,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnHorizontalOffsetPropertyChanged));

        /// <summary>
        /// Just a flag that the binding has been applied.
        /// </summary>
        private static readonly DependencyProperty HorizontalScrollBindingProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalScrollBinding",
                typeof(bool?),
                typeof(ScrollViewerBinding));

        public static double GetHorizontalOffset(DependencyObject depObj) =>
            (double)depObj.GetValue(HorizontalOffsetProperty);

        public static void SetHorizontalOffset(DependencyObject depObj, double value) =>
            depObj.SetValue(HorizontalOffsetProperty, value);

        private static void OnHorizontalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (double.IsNaN((double)e.NewValue)) return;
            if (!(d is ScrollViewer scrollViewer)) return;

            if (scrollViewer.GetValue(HorizontalScrollBindingProperty) == null)
            {
                scrollViewer.SetValue(HorizontalScrollBindingProperty, true);
                scrollViewer.ScrollChanged += ScrollViewer_HorizontalScrollChanged;
            }
            scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
        }

        // ◆イベント解除できておらずメモリリークの一因な気がする…
        private static void ScrollViewer_HorizontalScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(sender is ScrollViewer scrollViewer)) return;

            if (e.HorizontalChange != .0)
            {
                SetHorizontalOffset(scrollViewer, e.HorizontalOffset);
            }
        }

        #endregion

    }
}
