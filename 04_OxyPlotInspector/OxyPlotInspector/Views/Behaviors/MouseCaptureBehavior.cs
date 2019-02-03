using System.Windows;
using System.Windows.Input;

namespace OxyPlotInspector.Views.Behaviors
{
    class MouseCaptureBehavior : BehaviorBase<FrameworkElement>
    {
        protected override void OnSetup()
        {
            base.OnSetup();
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
        }

        protected override void OnCleanup()
        {
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
            base.OnCleanup();
        }

        /// <summary>
        /// マウス操作の強制キャプチャ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe)
                fe.CaptureMouse();
        }

        /// <summary>
        /// マウス操作の強制キャプチャを終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe)
                fe.ReleaseMouseCapture();
        }
    }
}
