using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ZoomThumb.Views.Behaviors
{
    public class MouseCaptureBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseButtonDown;
            AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseButtonUp;
            //AssociatedObject.MouseRightButtonDown += AssociatedObject_MouseButtonDown;
            //AssociatedObject.MouseRightButtonUp += AssociatedObject_MouseButtonUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseButtonDown;
            AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseButtonUp;
            //AssociatedObject.MouseRightButtonDown -= AssociatedObject_MouseButtonDown;
            //AssociatedObject.MouseRightButtonUp -= AssociatedObject_MouseButtonUp;
        }

        private void AssociatedObject_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement fe)) return;

            // コレが無いと素早い操作時に食み出て、マウスイベントを拾えなくなる(追従しない)
            fe.CaptureMouse();
        }

        private void AssociatedObject_MouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement fe)) return;

            // マウスの強制補足を終了
            fe.ReleaseMouseCapture();
        }

    }
}
