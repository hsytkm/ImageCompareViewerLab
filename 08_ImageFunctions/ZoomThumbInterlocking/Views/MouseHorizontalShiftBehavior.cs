using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using ZoomThumb.Views.Common;

namespace ZoomThumb.Views
{
    class MouseHorizontalShiftBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
        }

        /// <summary>
        /// MouseWheel+ShiftでScrollViewerを左右に移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                var scrollViewer = ViewHelper.GetChildControl<ScrollViewer>(sender);
                if (scrollViewer is null) return;

                if (e.Delta < 0)
                    scrollViewer.LineRight();
                else
                    scrollViewer.LineLeft();

                e.Handled = true;
            }
        }

    }
}
