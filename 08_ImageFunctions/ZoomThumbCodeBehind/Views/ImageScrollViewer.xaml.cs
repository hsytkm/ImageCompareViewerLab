using Reactive.Bindings;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZoomThumb.ViewModels;

namespace ZoomThumb.Views
{
    // 尾上さんのお言葉
    // Viewに特別な要件が存在しない限りは、トリガーやアクションの自作にこだわらず積極的にコードビハインドを使いましょう
    // Viewのコードビハインドは、基本的にView内で完結するロジックとViewModelからのイベントの受信(専用リスナを使用する)に限るとトラブルが少なくなります

    /// <summary>
    /// ImageScrollViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class ImageScrollViewer : UserControl
    {
        private static readonly ReactivePropertySlim<ImageZoomMagnification> ImageZoomMag = new ReactivePropertySlim<ImageZoomMagnification>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<Size> ScrollViewerSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<BitmapSource> ImageSource = new ReactivePropertySlim<BitmapSource>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<int> MouseWheelZoomDelta = new ReactivePropertySlim<int>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<Size> ImageScrollOffsetRatio = new ReactivePropertySlim<Size>(new Size(0.5, 0.5), mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);

        #region ZoomMagProperty

        private static readonly string ZoomMag = nameof(ZoomMag);

        private static readonly DependencyProperty ZoomMagProperty =
            DependencyProperty.RegisterAttached(
                nameof(ZoomMag),
                typeof(ImageZoomMagnification),
                typeof(ImageScrollViewer),
                new FrameworkPropertyMetadata(
                    ImageZoomMagnification.Entire,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnZoomMagPropertyChanged));

        public static ImageZoomMagnification GetImageZoomMag(DependencyObject depObj) =>
            (ImageZoomMagnification)depObj.GetValue(ZoomMagProperty);

        public static void SetZoomMag(DependencyObject depObj, ImageZoomMagnification value) =>
            depObj.SetValue(ZoomMagProperty, value);

        private static void OnZoomMagPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ImageZoomMagnification zoomMag) ImageZoomMag.Value = zoomMag;
        }

        #endregion

        #region ScrollOffsetProperty

        private static readonly string ScrollOffset = nameof(ScrollOffset);

        private static readonly DependencyProperty ScrollOffsetProperty =
            DependencyProperty.RegisterAttached(
                ScrollOffset,
                typeof(Size),
                typeof(ImageScrollViewer),
                new FrameworkPropertyMetadata(
                    new Size(),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) =>
                    {
                        // ViewModel→View
                        if (e.NewValue is Size size) ImageScrollOffsetRatio.Value = size;
                    }));

        public static Size GetScrollOffset(DependencyObject depObj) =>
            (Size)depObj.GetValue(ScrollOffsetProperty);

        public static void SetScrollOffset(DependencyObject depObj, Size value) =>
            depObj.SetValue(ScrollOffsetProperty, value);

        #endregion

        public ImageScrollViewer()
        {
            InitializeComponent();

            // ズーム表示に切り替え
            ImageZoomMag
                .CombineLatest(ImageSource, (mag, imageSource) => (mag, imageSource))
                .Where(x => !x.mag.IsEntire)
                .Subscribe(x =>
                {
                    var scrollViewer = MyScrollViewer;
                    var image = MainImage;

                    // ズーム表示からの全画面表示でスクロールバーが消えない対策
                    //   全画面表示中はスクロールバーを非表示にしているので、ズーム表示中は表示に戻す
                    scrollViewer.HorizontalScrollBarVisibility = 
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

                    image.Width = x.imageSource.PixelWidth * x.mag.MagnificationRatio;
                    image.Height = x.imageSource.PixelHeight * x.mag.MagnificationRatio;
                });

            // 全画面表示に切り替え
            ImageZoomMag
                .CombineLatest(ScrollViewerSize, ImageSource, (mag, sview, imageSource) => (mag, sview, imageSource))
                .Where(x => x.mag.IsEntire)
                .Subscribe(x =>
                {
                    var scrollViewer = MyScrollViewer;
                    var image = MainImage;

                    // ズーム表示からの全画面表示でスクロールバーが消えない対策
                    //   スクロールバーが表示された状態で、親コントロール(ScrollViewer)とジャストのサイズに設定すると、
                    //   自動でスクロールバーが消えないので明示的に消す
                    scrollViewer.HorizontalScrollBarVisibility = 
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

                    double width, height;
                    var imageRatio = (double)x.imageSource.PixelWidth / x.imageSource.PixelHeight;
                    if (imageRatio > x.sview.Width / x.sview.Height)
                    {
                        width = x.sview.Width;      // 横パンパン
                        height = x.sview.Width / imageRatio;
                    }
                    else
                    {
                        width = x.sview.Height * imageRatio;
                        height = x.sview.Height;    // 縦パンパン
                    }

                    image.Width = width;
                    image.Height = height;
                });

            // マウスホイールによるズーム倍率変更
            MouseWheelZoomDelta
                .CombineLatest(ImageSource, (delta, image) => (delta, image))
                .Where(x => x.delta != 0)
                .Subscribe(x =>
                {
                    var oldImageZoomMag = ImageZoomMag.Value;
                    var image = MainImage;
                    var isZoomIn = x.delta > 0;

                    // ズーム前の倍率
                    double oldZoomMagRatio = GetCurrentZoomMagnificationRatio(oldImageZoomMag, image, x.image);

                    // ズーム後のズーム管理クラス
                    var newImageZoomMag = oldImageZoomMag.ZoomMagnification(oldZoomMagRatio, isZoomIn);

                    // ズーム倍率の更新(オフセット更新前に実施)
                    ImageZoomMag.Value = newImageZoomMag;
                });

            // スクロールバーの移動
            ImageScrollOffsetRatio
                .CombineLatest(ScrollViewerSize, ImageZoomMag, (offset, sview, _) => (offset, sview, _))
                .Subscribe(x =>
                {
                    var scrollViewer = MyScrollViewer;
                    var image = MainImage;

                    var width = Math.Max(0.0, x.offset.Width * image.ActualWidth - (x.sview.Width / 2.0));
                    var height = Math.Max(0.0, x.offset.Height * image.ActualHeight - (x.sview.Height / 2.0));

                    scrollViewer.ScrollToHorizontalOffset(width);
                    scrollViewer.ScrollToVerticalOffset(height);

                    // View→ViewModelへの通知
                    SetScrollOffset(scrollViewer, x.offset);
                });

        }

        #region ViewImageSize

        private void MainImage_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (MainImage.Source is BitmapSource bitmap) ImageSource.Value = bitmap;
        }

        private void MyScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e) =>
            ScrollViewerSize.Value = e.NewSize;

        #endregion

        #region ImageSizeChanged

        private void MainImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var scrollViewer = MyScrollViewer;

            if (!(e.OriginalSource is Image image)) return;

            // ズーム倍率クラスの更新(TwoWay)
            UpdateImageZoomMag(scrollViewer, image);

            // e.NewSize == MainImage(.Size)
            ThumbCanvas.Visibility = (e.NewSize.Width > scrollViewer.ActualWidth || e.NewSize.Height > scrollViewer.ActualHeight)
                 ? Visibility.Visible : Visibility.Collapsed;

            UpdateBitmapScalingMode(image);
        }

        // ズーム倍率
        private static void UpdateImageZoomMag(ScrollViewer scrollViewer, Image image)
        {
            if (!(image.Source is BitmapSource imageSource)) return;

            var currentMag = ImageZoomMag.Value;
            var ratio = GetCurrentZoomMagnificationRatio(currentMag, image, imageSource);
            var mag = new ImageZoomMagnification(currentMag.IsEntire, ratio);

            ImageZoomMag.Value = mag;
            SetZoomMag(scrollViewer, mag);
        }

        // レンダリングオプションの指定(100%以上の拡大ズームならPixelが見える設定にする)
        private static void UpdateBitmapScalingMode(Image image)
        {
            if (!(image.Source is BitmapSource bitmap)) return;

            var mode = (image.ActualWidth < bitmap.PixelWidth) || (image.ActualHeight < bitmap.PixelHeight)
                ? BitmapScalingMode.HighQuality : BitmapScalingMode.NearestNeighbor;
            RenderOptions.SetBitmapScalingMode(image, mode);
        }

        #endregion

        #region MouseWheelZoom

        // ThumbCanvasのPreviewMouseWheelはMyScrollViewerに委託
        private void ThumbCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e) =>
            MyScrollViewer_PreviewMouseWheel(sender, e);

        private void MyScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                MouseWheelZoomDelta.Value = e.Delta;

                // 最大ズームでホイールすると画像の表示エリアが移動しちゃうので止める
                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                if (!(sender is ScrollViewer scrview)) return;

                if (e.Delta < 0)
                    scrview.LineRight();
                else
                    scrview.LineLeft();

                e.Handled = true;
            }
        }

        // 現在の画像のズーム倍率を返す
        private static double GetCurrentZoomMagnificationRatio(ImageZoomMagnification imageZoomMag, Image image, BitmapSource imageSource)
        {
            // 全画面表示でなければ倍率が入っているのでそのまま返す
            if (!imageZoomMag.IsEntire) return imageZoomMag.MagnificationRatio;

            return Math.Min(image.ActualWidth / imageSource.PixelWidth, image.ActualHeight / imageSource.PixelHeight);
        }

        #endregion

        #region ThumbnailViewport

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            var scrollViewer = MyScrollViewer;
            var image = MainImage;
            var thumbnail = Thumbnail;

            var width = scrollViewer.HorizontalOffset + (e.HorizontalChange * scrollViewer.ExtentWidth / thumbnail.ActualWidth);
            width += scrollViewer.ActualWidth / 2.0;
            width = clip(Math.Round(width), 0.0, image.ActualWidth);

            var height = scrollViewer.VerticalOffset + (e.VerticalChange * scrollViewer.ExtentHeight / thumbnail.ActualHeight);
            height += scrollViewer.ActualHeight / 2.0;
            height = clip(Math.Round(height), 0.0, image.ActualHeight);

            // スクロール位置を更新
            ImageScrollOffsetRatio.Value = new Size(width / image.ActualWidth, height / image.ActualHeight);
        }

        private void UpdateThumbnailViewport(object sender, ScrollChangedEventArgs e)
        {
            var thumbnail = Thumbnail;
            var thumbViewport = ThumbViewport;
            var combinedGeometry = CombinedGeometry;

            // ExtentWidth/Height が ScrollViewer 内の広さ
            // ViewportWidth/Height が ScrollViewer で実際に表示されているサイズ

            var xfactor = thumbnail.ActualWidth / e.ExtentWidth;
            var yfactor = thumbnail.ActualHeight / e.ExtentHeight;

            var left = e.HorizontalOffset * xfactor;
            var top = e.VerticalOffset * yfactor;

            var width = e.ViewportWidth * xfactor;
            if (width > thumbnail.ActualWidth) width = thumbnail.ActualWidth;

            var height = e.ViewportHeight * yfactor;
            if (height > thumbnail.ActualHeight) height = thumbnail.ActualHeight;

            Canvas.SetLeft(thumbViewport, left);
            Canvas.SetTop(thumbViewport, top);
            thumbViewport.Width = width;
            thumbViewport.Height = height;

            combinedGeometry.Geometry2 = new RectangleGeometry(new Rect(left, top, width, height));
        }

        #endregion

    }
}
