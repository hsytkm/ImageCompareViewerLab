using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        // スクロールバー位置の初期値
        private static readonly Size DefaultScrollOffsetRatio = new Size(0.5, 0.5);

        private static readonly ReactivePropertySlim<ImageZoomMagnification> ImageZoomMag = new ReactivePropertySlim<ImageZoomMagnification>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<Size> ScrollViewerSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<BitmapSource> ImageSource = new ReactivePropertySlim<BitmapSource>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<int> MouseWheelZoomDelta = new ReactivePropertySlim<int>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<Size> ImageScrollOffsetRatio = new ReactivePropertySlim<Size>(DefaultScrollOffsetRatio, mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);

        private static readonly ReactivePropertySlim<Unit> ScrollViewerMouseLeftDown = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<Unit> ScrollViewerMouseLeftUp = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<Point> ScrollViewerMouseMove = new ReactivePropertySlim<Point>();

        #region ZoomMagProperty

        private static readonly string ZoomMag = nameof(ZoomMag);

        private static readonly DependencyProperty ZoomMagProperty =
            DependencyProperty.RegisterAttached(
                nameof(ZoomMag),
                typeof(ImageZoomMagnification),
                typeof(ImageScrollViewer),
                new FrameworkPropertyMetadata(
                    default(ImageZoomMagnification),
                    //FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnZoomMagPropertyChanged));

        public static ImageZoomMagnification GetImageZoomMag(DependencyObject depObj) =>
            (ImageZoomMagnification)depObj.GetValue(ZoomMagProperty);

        public static void SetZoomMag(DependencyObject depObj, ImageZoomMagnification value) =>
            depObj.SetValue(ZoomMagProperty, value);

        private static void OnZoomMagPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (e.NewValue is ImageZoomMagnification zoomMag) ImageZoomMag.Value = zoomMag;
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
                    default(Size),
                    //FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) =>
                    {
                        // ViewModel→View
                        //if (e.NewValue is Size size) ImageScrollOffsetRatio.Value = size;
                    }));

        public static Size GetScrollOffset(DependencyObject depObj) =>
            (Size)depObj.GetValue(ScrollOffsetProperty);

        public static void SetScrollOffset(DependencyObject depObj, Size value) =>
            depObj.SetValue(ScrollOffsetProperty, value);

        #endregion

        public ImageScrollViewer()
        {
            InitializeComponent();

            #region EventHandlers

            MyScrollViewer.Loaded += (_, __) =>
            {
                // ScrollContentPresenterが生成されてから設定
                var scrollContentPresenter = GetChildControl<ScrollContentPresenter>(MyScrollViewer);
                scrollContentPresenter.PreviewMouseLeftButtonDown += (sender, e) => ScrollViewerMouseLeftDown.Value = Unit.Default;
                scrollContentPresenter.PreviewMouseLeftButtonUp += (sender, e) => ScrollViewerMouseLeftUp.Value = Unit.Default;
                scrollContentPresenter.MouseMove += (sender, e) => ScrollViewerMouseMove.Value = e.GetPosition((IInputElement)sender);
            };

            MyScrollViewer.PreviewMouseWheel += new MouseWheelEventHandler(MyScrollViewer_PreviewMouseWheel);
            MyScrollViewer.SizeChanged += (sender, e) => ScrollViewerSize.Value = e.NewSize;
            MyScrollViewer.ScrollChanged += new ScrollChangedEventHandler(MyScrollViewer_ScrollChanged);

            MainImage.TargetUpdated += (sender, e) =>
            {
                var source = RoutedEventArgsToBiatmapSource(e);
                if (source is null) return;

                // 初回画像は全画面表示
                bool isFirstTime = (ImageSource.Value is null);

                ImageSource.Value = source;
                if (isFirstTime) ImageZoomMag.Value = ImageZoomMagnification.Entire;
            };
            MainImage.SizeChanged += new SizeChangedEventHandler(MainImage_SizeChanged);

            // ThumbCanvasのPreviewMouseWheelはMyScrollViewerに委託  ◆よりスマートな記述ありそう
            ThumbCanvas.PreviewMouseWheel += new MouseWheelEventHandler(MyScrollViewer_PreviewMouseWheel);

            ThumbViewport.DragDelta += new DragDeltaEventHandler(OnDragDelta);

            #endregion

            // ダブルクリックイベントの自作 http://y-maeyama.hatenablog.com/entry/20110313/1300002095
            // ScrollViewerのMouseDoubleClickだとScrollBarのDoubleClickも拾ってしまうので
            // またScrollContentPresenterにMouseDoubleClickイベントは存在しない
            ScrollViewerMouseLeftDown
                .Select(_ => DateTime.Now)
                .Scan((prev, current) => current.Subtract(prev) > TimeSpan.FromMilliseconds(500) ? current : DateTime.MinValue)
                .Where(t => t == DateTime.MinValue)
                .Subscribe(_ => SwitchClickZoomMag());

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

                    var width = Math.Max(0.0, x.offset.Width * image.Width - (x.sview.Width / 2.0));
                    var height = Math.Max(0.0, x.offset.Height * image.Height - (x.sview.Height / 2.0));

                    scrollViewer.ScrollToHorizontalOffset(width);
                    scrollViewer.ScrollToVerticalOffset(height);

                    // View→ViewModelへの通知
                    SetScrollOffset(scrollViewer, x.offset);
                });

            // ドラッグによる画像表示領域の移動
            ScrollViewerMouseMove
                .Pairwise()                                         // 最新値と前回値を取得
                .Select(x => -(x.NewItem - x.OldItem))              // 引っ張りと逆方向なので反転
                .SkipUntil(ScrollViewerMouseLeftDown)
                .TakeUntil(ScrollViewerMouseLeftUp)
                .Repeat()
                .Where(_ => !ImageZoomMag.Value.IsEntire)           // ズーム中のみ流す(全画面表示中は画像移動不要)
                                                                    //.Where(_ => !temporaryZoom.Value)                   // ◆一時ズームは移動させない仕様
                .Subscribe(shift =>
                {
                    double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

                    var image = MainImage;

                    // マウスドラッグ中の表示位置のシフト
                    var shiftRate = new Vector(shift.X / image.Width, shift.Y / image.Height);

                    ImageScrollOffsetRatio.Value = new Size(
                        clip(ImageScrollOffsetRatio.Value.Width + shiftRate.X, 0.0, 1.0),
                        clip(ImageScrollOffsetRatio.Value.Height + shiftRate.Y, 0.0, 1.0));
                });

        }

        private static T GetChildControl<T>(DependencyObject d) where T : DependencyObject
        {
            if (d is T control) return control;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                control = GetChildControl<T>(VisualTreeHelper.GetChild(d, i));
                if (control != default) return control;
            }
            return default;
        }

        #region ImageSizeChanged

        // 画像のサイズ変更(ズーム操作)
        private void MainImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!(e.OriginalSource is Image image)) return;

            var scrollViewer = MyScrollViewer;

            // 全画面表示よりもズームしてるかフラグ(e.NewSize == Size of MainImage)
            bool isZoomOverEntire = (e.NewSize.Width > scrollViewer.ActualWidth || e.NewSize.Height > scrollViewer.ActualHeight);

            // ズーム倍率クラスの更新(TwoWay)
            UpdateImageZoomMag(scrollViewer, image);

            // 全画面よりズームインしてたらサムネイル
            ThumbCanvas.Visibility = isZoomOverEntire ? Visibility.Visible : Visibility.Collapsed;

            // 全画面よりズームアウトしたらスクロールバー位置を初期化
            if (!isZoomOverEntire) ImageScrollOffsetRatio.Value = DefaultScrollOffsetRatio;

            UpdateBitmapScalingMode(image);
        }

        // ズーム倍率を設定(ViewModelに全画面表示の倍率を通知するため)
        private static void UpdateImageZoomMag(ScrollViewer scrollViewer, Image image)
        {
            if (!(image.Source is BitmapSource imageSource)) return;

            if (ImageZoomMag.Value.IsEntire)
            {
                var entireRatio = GetCurrentZoomMagnificationRatio(ImageZoomMag.Value, image, imageSource);
                ImageZoomMag.Value.SetsEntireMagnificationRatio(entireRatio);
            }
            SetZoomMag(scrollViewer, ImageZoomMag.Value);
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

            //Console.WriteLine($"Delta: {width} x {height}");

            // スクロール位置を更新
            ImageScrollOffsetRatio.Value = new Size(width / image.ActualWidth, height / image.ActualHeight);
        }

        private void MyScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // ◆ここでスクロールバーの操作をプロパティに通知せなアカン
            //   ImageScrollOffsetRatioを更新せなアカン

            UpdateThumbnailViewport(sender, e);
        }

        private void UpdateThumbnailViewport(object sender, ScrollChangedEventArgs e)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            var thumbnail = Thumbnail;
            var thumbViewport = ThumbViewport;
            var combinedGeometry = CombinedGeometry;

            // ExtentWidth/Height が ScrollViewer 内の広さ
            // ViewportWidth/Height が ScrollViewer で実際に表示されているサイズ

            var xfactor = thumbnail.ActualWidth / e.ExtentWidth;
            var yfactor = thumbnail.ActualHeight / e.ExtentHeight;

            var left = e.HorizontalOffset * xfactor;
            left = clip(left, 0.0, thumbnail.ActualWidth - thumbViewport.MinWidth);

            var top = e.VerticalOffset * yfactor;
            top = clip(top, 0.0, thumbnail.ActualHeight - thumbViewport.MinHeight);

            var width = e.ViewportWidth * xfactor;
            width = clip(width, thumbViewport.MinWidth, thumbnail.ActualWidth);

            var height = e.ViewportHeight * yfactor;
            height = clip(height, thumbViewport.MinHeight, thumbnail.ActualHeight);

            Canvas.SetLeft(thumbViewport, left);
            Canvas.SetTop(thumbViewport, top);
            thumbViewport.Width = width;
            thumbViewport.Height = height;

            combinedGeometry.Geometry2 = new RectangleGeometry(new Rect(left, top, width, height));
        }

        #endregion

        // クリックズームの状態を切り替える(全画面⇔ズーム)
        private void SwitchClickZoomMag()
        {
            if (ImageZoomMag.Value.IsEntire)
            {
                ImageZoomMag.Value = ImageZoomMagnification.MagX1;  // ToZoom

            }
            else
            {
                //ここで倍率詰めるのは無理(コントロールサイズが変わっていないため)
                ImageZoomMag.Value = ImageZoomMagnification.Entire; // ToAll
            }

            // ズーム表示への切り替えならスクロールバーを移動(ImageViewSizeを変更した後に実施する)
            if (!ImageZoomMag.Value.IsEntire)
            {
                double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

                var imageViewSize = new Size(MainImage.ActualWidth, MainImage.ActualHeight);
                var scrollViewerSize = ScrollViewerSize.Value;
                var imageSourceSize = new Size(ImageSource.Value.PixelWidth, ImageSource.Value.PixelHeight);
                var scrollVieweMousePoint = ScrollViewerMouseMove.Value;

                // 親ScrollViewerから子Imageまでのサイズ
                var imageControlSizeOffset = new Size(
                    Math.Max(0, scrollViewerSize.Width - imageViewSize.Width) / 2.0,
                    Math.Max(0, scrollViewerSize.Height - imageViewSize.Height) / 2.0);

                // 子Image基準のマウス位置
                var mousePos = new Point(
                    Math.Max(0, scrollVieweMousePoint.X - imageControlSizeOffset.Width),
                    Math.Max(0, scrollVieweMousePoint.Y - imageControlSizeOffset.Height));

                // ズーム後の中心座標の割合
                var zoomCenterRate = new Size(
                    clip(mousePos.X / imageViewSize.Width, 0.0, 1.0),
                    clip(mousePos.Y / imageViewSize.Height, 0.0, 1.0));

                ImageScrollOffsetRatio.Value = zoomCenterRate;
            }
        }

        private static BitmapSource RoutedEventArgsToBiatmapSource(RoutedEventArgs e)
        {
            if (!(e.OriginalSource is Image image)) return null;
            if (!(image.Source is BitmapSource source)) return null;
            return source;
        }

    }
}
