using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZoomThumb.Views.Common;

namespace ZoomThumb.Views.Controls
{
    /// <summary>
    /// ReducedImageCanvas.xaml の相互作用ロジック
    /// </summary>
    public partial class ReducedImageCanvas : UserControl
    {
        // 無理やりコントロールを取得（良くない）
        private ScrollViewer ScrollViewer;
        private Image MainImage;

        #region ScrollOffsetCenterRatioRequestProperty(OneWayToSource)

        private static readonly DependencyProperty ScrollOffsetCenterRatioRequestProperty =
            DependencyProperty.RegisterAttached(
                nameof(ScrollOffsetCenterRatioRequest),
                typeof(Size),
                typeof(ReducedImageCanvas));

        public Size ScrollOffsetCenterRatioRequest
        {
            get => (Size)GetValue(ScrollOffsetCenterRatioRequestProperty);
            set => SetValue(ScrollOffsetCenterRatioRequestProperty, value);
        }

        #endregion

        public ReducedImageCanvas()
        {
            InitializeComponent();

            this.Loaded += (_, __) =>
            {
                var scrollViewer = ViewHelper.GetChildControl<ScrollViewer>(this.Parent);
                if (scrollViewer != null)
                {
                    // 主画像のスクロール時にViewportを更新する
                    // +=とAddHandlerで同じ動作っぽい(stackoverflow) https://stackoverflow.com/questions/2146982/uielement-addhandler-vs-event-definition
                    //scrollViewer?.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(UpdateThumbnailViewport));
                    scrollViewer.ScrollChanged += UpdateThumbnailViewport;
                    ScrollViewer = scrollViewer;
                }

                var mainImage = ViewHelper.GetChildControl<Image>(scrollViewer);
                if (mainImage != null)
                {
                    // AddHandlerでの実装方法が分からなかった
                    mainImage.TargetUpdated += ThumbImage_TargetUpdated;
                    MainImage = mainImage;
                }

            };

            ThumbViewport.DragDelta += new DragDeltaEventHandler(OnDragDelta);
        }

        // 主画像の更新時に縮小画像も更新
        private void ThumbImage_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (!(e.OriginalSource is Image image)) return;
            if (!(image?.Source is BitmapSource source)) return;
            ThumbImage.Source = source;
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            var scrollViewer = ScrollViewer;
            var mainImage = MainImage;
            if (scrollViewer == null || mainImage == null) return;

            var scrollViewerActualSize = ViewHelper.GetControlActualSize(scrollViewer);
            if (!scrollViewerActualSize.IsValidValue()) return;

            var mainImageActualSize = ViewHelper.GetControlActualSize(mainImage);
            if (!mainImageActualSize.IsValidValue()) return;

            var thumbImageActualSize = ViewHelper.GetControlActualSize(ThumbImage);
            if (!thumbImageActualSize.IsValidValue()) return;

            var width = scrollViewer.HorizontalOffset + (e.HorizontalChange * scrollViewer.ExtentWidth / thumbImageActualSize.Width);
            width += scrollViewerActualSize.Width / 2.0;
            width /= mainImageActualSize.Width;
            width = clip(width, 0.0, 1.0);

            var height = scrollViewer.VerticalOffset + (e.VerticalChange * scrollViewer.ExtentHeight / thumbImageActualSize.Height);
            height += scrollViewerActualSize.Height / 2.0;
            height /= mainImageActualSize.Height;
            height = clip(height, 0.0, 1.0);

            // スクロール位置の更新依頼(厳密な範囲制限はScrollViewer内で行ってもらう)
            ScrollOffsetCenterRatioRequest = new Size(width, height);
        }

        // 主画像のスクロール時にViewportを更新する
        private void UpdateThumbnailViewport(object sender, ScrollChangedEventArgs e)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            var thumbViewport = ThumbViewport;

            var thumbImageActualSize = ViewHelper.GetControlActualSize(ThumbImage);
            if (!thumbImageActualSize.IsValidValue()) return;

            // ExtentWidth/Height が ScrollViewer 内の広さ
            // ViewportWidth/Height が ScrollViewer で実際に表示されているサイズ

            if (!e.ExtentWidth.IsValidValue() || !e.ExtentHeight.IsValidValue()) return;
            var xfactor = thumbImageActualSize.Width / e.ExtentWidth;
            var yfactor = thumbImageActualSize.Height / e.ExtentHeight;

            var left = e.HorizontalOffset * xfactor;
            left = clip(left, 0.0, thumbImageActualSize.Width - thumbViewport.MinWidth);

            var top = e.VerticalOffset * yfactor;
            top = clip(top, 0.0, thumbImageActualSize.Height - thumbViewport.MinHeight);

            var width = e.ViewportWidth * xfactor;
            width = clip(width, thumbViewport.MinWidth, thumbImageActualSize.Width);

            var height = e.ViewportHeight * yfactor;
            height = clip(height, thumbViewport.MinHeight, thumbImageActualSize.Height);

            Canvas.SetLeft(thumbViewport, left);
            Canvas.SetTop(thumbViewport, top);
            thumbViewport.Width = width;
            thumbViewport.Height = height;

            CombinedGeometry.Geometry2 = new RectangleGeometry(new Rect(left, top, width, height));
        }

    }
}
