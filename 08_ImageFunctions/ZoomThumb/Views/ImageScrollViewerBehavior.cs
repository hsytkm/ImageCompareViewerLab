using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ZoomThumb.Views
{
    /// <summary>
    /// 画像ScrollViewer専用
    /// </summary>
    class ImageScrollViewerBehavior : MouseCaptureBehavior
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
            //AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
            //AssociatedObject.TargetUpdated += AssociatedObject_TargetUpdated;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
            //AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
            //AssociatedObject.TargetUpdated -= AssociatedObject_TargetUpdated;
            base.OnDetaching();
        }

        /// <summary>
        /// Shift押下時は水平方向にシフト
        /// https://stackoverflow.com/questions/3727439/how-to-enable-horizontal-scrolling-with-mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!(sender is ScrollViewer scrview)) return;
            if (Keyboard.Modifiers != ModifierKeys.Shift) return;

            if (e.Delta < 0)
                scrview.LineRight();
            else
                scrview.LineLeft();

            e.Handled = true;
        }

#if false
        /// <summary>
        /// ScrollViewerのサイズ変更時にScrollBarの表示を更新
        /// (画像がちょっと食み出てバー出てる状態から親サイズ広げたらバー消えるように)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!(sender is ScrollViewer scrview)) return;

            // スクロールバーの表示を更新
            UpdateScrollBarVisibility(scrview);
        }

        /// <summary>
        /// ScrollViewerの子Imageが変化したときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (!(sender is ScrollViewer scrview)) return;

            // スクロールバーの表示を更新
            UpdateScrollBarVisibility(scrview);
        }

        /// <summary>
        /// スクロールバーの表示を更新
        /// </summary>
        /// <param name="scrview"></param>
        private static void UpdateScrollBarVisibility(ScrollViewer scrview)
        {
            var image = GetImageControl(scrview);
            if (image == null) return;

            // 画像を拡大した状態から全画面に戻すとスクロールバーが消えない対策
            scrview.HorizontalScrollBarVisibility = (scrview.ActualWidth >= image.ActualWidth)
                ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Auto;
            scrview.VerticalScrollBarVisibility = (scrview.ActualHeight >= image.ActualHeight)
                ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Auto;
        }

        /// <summary>
        /// 基準コントロールから子Imageコントロールを探索して取得
        /// </summary>
        /// <param name="scrview"></param>
        /// <returns></returns>
        private static Image GetImageControl(DependencyObject d)
        {
            if (d is Image image) return image;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                image = GetImageControl(VisualTreeHelper.GetChild(d, i));
                if (image != null) return image;
            }
            return null;
        }
#endif

    }
}
