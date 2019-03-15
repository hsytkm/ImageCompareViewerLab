using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace ZoomThumb.Views
{
    /// <summary>
    /// スクロールバーの非表示要求
    /// </summary>
    class ScrollBarBehavior : Behavior<ScrollBar>
    {
        public static readonly DependencyProperty ForceVisibilityCollapsedProperty =
            DependencyProperty.Register(
                nameof(ForceVisibilityCollapsed),
                typeof(bool),
                typeof(ScrollBarBehavior),
                new FrameworkPropertyMetadata(
                    false,
                    OnForceVisibilityCollapsedPropertyChanged));

        public bool ForceVisibilityCollapsed
        {
            get => (bool)GetValue(ForceVisibilityCollapsedProperty);
            set => SetValue(ForceVisibilityCollapsedProperty, value);
        }

        private static void OnForceVisibilityCollapsedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollBarBehavior b)
            {
                if (b.AssociatedObject is ScrollBar sbar && e.NewValue is bool f)
                {
                    sbar.Visibility = f ? Visibility.Collapsed: Visibility.Visible;
                }
            }
        }

    }
}
