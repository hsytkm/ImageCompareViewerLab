using System;
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
        private static readonly Type SelfType = typeof(ReducedImageCanvas);

        #region ScrollOffsetVectorRatioRequestProperty(OneWayToSource)

        private static readonly DependencyProperty ScrollOffsetVectorRatioRequestProperty =
            DependencyProperty.Register(
                nameof(ScrollOffsetVectorRatioRequest),
                typeof(Vector),
                SelfType);

        public Vector ScrollOffsetVectorRatioRequest
        {
            get => (Vector)GetValue(ScrollOffsetVectorRatioRequestProperty);
            set => SetValue(ScrollOffsetVectorRatioRequestProperty, value);
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
                    // +=とAddHandlerで同じ動作っぽい(stackoverflow) https://stackoverflow.com/questions/2146982/uielement-addhandler-vs-event-definition
                    //scrollViewer?.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(UpdateThumbnailViewport));
                    scrollViewer.ScrollChanged += UpdateThumbnailViewport;
                }

                var mainImage = ViewHelper.GetChildControl<Image>(scrollViewer);
                if (mainImage != null)
                {
                    // AddHandlerでの実装方法が分からなかった
                    mainImage.TargetUpdated += ThumbImage_TargetUpdated;
                }
            };

            ThumbViewport.DragDelta += new DragDeltaEventHandler(OnDragDelta);
        }

        // 主画像の更新時に縮小画像も連動して更新
        private void ThumbImage_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (!(e.OriginalSource is Image image)) return;
            if (!(image?.Source is BitmapSource source)) return;
            ThumbImage.Source = source;
        }

        // 縮小画像のドラッグ操作を主画像に伝える
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumbImageActualSize = ViewHelper.GetControlActualSize(ThumbImage);
            if (!thumbImageActualSize.IsValidValue()) return;

            // スクロール位置の変化割合を通知
            ScrollOffsetVectorRatioRequest = new Vector(
                e.HorizontalChange / thumbImageActualSize.Width,
                e.VerticalChange / thumbImageActualSize.Height);
        }

        // 主画像のスクロール更新時にViewportを更新する
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
