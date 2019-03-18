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
        private static readonly ReactivePropertySlim<int> MouseWheelDelta = new ReactivePropertySlim<int>(mode: ReactivePropertyMode.None);

        // ◆TwoWayのToSource側が未実装
        private static readonly ReactivePropertySlim<Size> ImageScrollOffset = new ReactivePropertySlim<Size>(new Size(0.5, 0.5));

        #region ImageZoomMagProperty

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
                        if (e.NewValue is Size size) ImageScrollOffset.Value = size;
                    }));

        public static Size GetScrollOffset(DependencyObject depObj) =>
            (Size)depObj.GetValue(ScrollOffsetProperty);

        public static void SetScrollOffset(DependencyObject depObj, object value) =>
            depObj.SetValue(ScrollOffsetProperty, value);

        #endregion

        public ImageScrollViewer()
        {
            InitializeComponent();

            // ズーム表示に切り替え
            ImageZoomMag
                .CombineLatest(ImageSource, (mag, image) => (mag, image))
                .Where(x => !x.mag.IsEntire)
                .Subscribe(x =>
                {
                    // ズーム表示からの全画面表示でスクロールバーが消えない対策
                    //   全画面表示中はスクロールバーを非表示にしているので、ズーム表示中は表示に戻す
                    MyScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    MyScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

                    var size = new Size(x.image.PixelWidth * x.mag.MagnificationRatio, x.image.PixelHeight * x.mag.MagnificationRatio);

                    UpdateImageSize(MainImage, size, x.mag);    //ToZoom
                });

            // 全画面表示に切り替え
            ImageZoomMag
                .CombineLatest(ScrollViewerSize, ImageSource, (mag, sview, image) => (mag, sview, image))
                .Where(x => x.mag.IsEntire)
                .Subscribe(x =>
                {
                    // ズーム表示からの全画面表示でスクロールバーが消えない対策
                    //   スクロールバーが表示された状態で、親コントロール(ScrollViewer)とジャストのサイズに設定すると、
                    //   自動でスクロールバーが消えないので明示的に消す
                    MyScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    MyScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

                    Size size;
                    var imageRatio = (double)x.image.PixelWidth / x.image.PixelHeight;
                    if (imageRatio > x.sview.Width / x.sview.Height)
                    {
                        size = new Size(x.sview.Width, x.sview.Width / imageRatio);     // 横パンパン
                    }
                    else
                    {
                        size = new Size(x.sview.Height * imageRatio, x.sview.Height);   // 縦パンパン
                    }

                    UpdateImageSize(MainImage, size, x.mag);    //ToAll
                });

            // マウスホイールによるズーム倍率変更
            MouseWheelDelta
                .CombineLatest(ScrollViewerSize, ImageSource, (delta, sview, image) => (delta, sview, image))
                .Where(x => x.delta != 0)
                .Subscribe(x =>
                {
                    // 現在のスクロールバーの中央位置
                    var scrollOffsetCenterRatio = ImageScrollOffset.Value;

                    var imageViewSize = new Size(MainImage.Width, MainImage.Height);
                    var oldImageZoomMag = ImageZoomMag.Value;
                    var scrollViewer = MyScrollViewer;

                    var isZoomIn = x.delta > 0;
                    var scrollViewerSize = x.sview;
                    var imageSourceSize = new Size(x.image.PixelWidth, x.image.PixelHeight);

                    // ズーム前の倍率
                    double oldZoomMagRatio = GetCurrentZoomMagnificationRatio(oldImageZoomMag, imageViewSize, imageSourceSize);

                    // ズーム後のズーム管理クラス
                    var newImageZoomMag = oldImageZoomMag.ZoomMagnification(oldZoomMagRatio, isZoomIn);

                    // ズーム後の画像サイズ
                    var newImageSize = new Size(
                        imageViewSize.Width * newImageZoomMag.MagnificationRatio / oldZoomMagRatio,
                        imageViewSize.Height * newImageZoomMag.MagnificationRatio / oldZoomMagRatio);

                    // scrollOffsetCenterRatioが0~1の範囲内なので最大側は制限不要(値に変化ある場合のみ設定)
                    var newOffset = new Size(
                        Math.Max(0.0, scrollOffsetCenterRatio.Width * newImageSize.Width - (scrollViewerSize.Width / 2.0)),
                        Math.Max(0.0, scrollOffsetCenterRatio.Height * newImageSize.Height - (scrollViewerSize.Height / 2.0)));

                    scrollViewer.ScrollToHorizontalOffset(newOffset.Width);
                    scrollViewer.ScrollToVerticalOffset(newOffset.Height);

                    // ズーム倍率の更新(オフセット更新後に実施)
                    ImageZoomMag.Value = newImageZoomMag;

                    Console.WriteLine($"CodeBehind_Mag: {oldZoomMagRatio:f2} => {newImageZoomMag.MagnificationRatio:f2}");
                });

            // スクロールバーの移動
            ImageScrollOffset
                .CombineLatest(ScrollViewerSize, (offset, sview) => (offset, sview))
                .Subscribe(x =>
                {
                    var imageViewSize = new Size(MainImage.Width, MainImage.Height);
                    var scrollViewer = MyScrollViewer;

                    if (double.IsNaN(imageViewSize.Width) || double.IsNaN(imageViewSize.Height)) return;

                    var newOffset = new Size(
                        Math.Max(0.0, x.offset.Width * imageViewSize.Width - (x.sview.Width / 2.0)),
                        Math.Max(0.0, x.offset.Height * imageViewSize.Height - (x.sview.Height / 2.0)));

                    scrollViewer.ScrollToHorizontalOffset(newOffset.Width);
                    scrollViewer.ScrollToVerticalOffset(newOffset.Height);
                });
        }

        #region ViewImageSize

        private void MainImage_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (MainImage.Source is BitmapSource bitmap) ImageSource.Value = bitmap;
        }

        private void MyScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e) =>
            ScrollViewerSize.Value = e.NewSize;

        private void UpdateImageSize(Image image, Size size, ImageZoomMagnification mag)
        {
            SetZoomMag(MyScrollViewer, mag);

            image.Width = size.Width;
            image.Height = size.Height;

            UpdateBitmapScalingMode(image);
        }

        #endregion

        #region BitmapScalingMode

        private void MainImage_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateBitmapScalingMode(MainImage);

        // レンダリングオプションの指定(100%以上の拡大ズームならPixelが見える設定にする)
        private static void UpdateBitmapScalingMode(Image image)
        {
            if (!(image.Source is BitmapSource bitmap)) return;

            var mode = (image.Width < image.Source.Width) || (image.Height < image.Source.Height)
                ? BitmapScalingMode.HighQuality : BitmapScalingMode.NearestNeighbor;
            RenderOptions.SetBitmapScalingMode(image, mode);
        }

        #endregion

        #region MouseWheelZoom

        private void MyScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                MouseWheelDelta.Value = e.Delta;

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
        private static double GetCurrentZoomMagnificationRatio(ImageZoomMagnification imageZoomMag, Size imageViewSize, Size imageSourceSize)
        {
            // 全画面表示でなければ倍率が入っているのでそのまま返す
            if (!imageZoomMag.IsEntire) return imageZoomMag.MagnificationRatio;

            return Math.Min(
                imageViewSize.Width / imageSourceSize.Width,
                imageViewSize.Height / imageSourceSize.Height);
        }

        #endregion

        #region ThumbnailViewport

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumbnail = Thumbnail;
            var scrollViewer = MyScrollViewer;

            scrollViewer.ScrollToHorizontalOffset(
                scrollViewer.HorizontalOffset + (e.HorizontalChange * scrollViewer.ExtentWidth / thumbnail.ActualWidth));

            MyScrollViewer.ScrollToVerticalOffset(
                scrollViewer.VerticalOffset + (e.VerticalChange * scrollViewer.ExtentHeight / thumbnail.ActualHeight));
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

            // Canvas (親パネル) 上での Thumb の位置を、Left/Top 添付プロパティで設定
            // (XAML で言う <Thumb Canvas.Left="0" ... \> みたいなやつ)
            Canvas.SetLeft(thumbViewport, left);
            Canvas.SetTop(thumbViewport, top);

            thumbViewport.Width = width;
            thumbViewport.Height = height;

            combinedGeometry.Geometry2 = new RectangleGeometry(new Rect(left, top, width, height));
        }

        #endregion

    }
}
