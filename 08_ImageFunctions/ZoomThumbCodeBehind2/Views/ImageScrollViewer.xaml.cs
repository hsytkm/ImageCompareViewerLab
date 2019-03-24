using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Diagnostics;
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
        // スクロールバー位置の初期値
        private static readonly Size DefaultScrollOffsetRatio = new Size(0.5, 0.5);

        private static readonly ReactivePropertySlim<ImageZoomMagnification> ImageZoomMag = new ReactivePropertySlim<ImageZoomMagnification>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private static readonly ReactivePropertySlim<Size> ScrollContentActualSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private static readonly ReactivePropertySlim<Size> ImageViewActualSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private static readonly ReactivePropertySlim<Size> ImageSourcePixelSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private static readonly ReactivePropertySlim<BitmapSource> ImageSource = new ReactivePropertySlim<BitmapSource>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private static readonly ReactivePropertySlim<int> MouseWheelZoomDelta = new ReactivePropertySlim<int>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<Size> ImageScrollOffsetRatio = new ReactivePropertySlim<Size>(DefaultScrollOffsetRatio, mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);

        private static readonly ReactivePropertySlim<Unit> ScrollContentMouseLeftDown = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<Unit> ScrollContentMouseLeftUp = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None);
        private static readonly ReactivePropertySlim<Point> ScrollContentMouseMove = new ReactivePropertySlim<Point>(mode: ReactivePropertyMode.None);
        //private static readonly ReactivePropertySlim<Unit> ScrollContentDoubleClick = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None);

        private ScrollChangedEventArgs ScrollChangedEventInfo;

        #region ZoomPayloadProperty

        private static readonly string ZoomPayload = nameof(ZoomPayload);

        private static readonly DependencyProperty ZoomPayloadProperty =
            DependencyProperty.RegisterAttached(
                nameof(ZoomPayload),
                typeof(ImageZoomPayload),
                typeof(ImageScrollViewer),
                new FrameworkPropertyMetadata(default(ImageZoomPayload)));

        public static ImageZoomPayload GetZoomPayload(DependencyObject depObj) =>
            (ImageZoomPayload)depObj.GetValue(ZoomPayloadProperty);

        public static void SetZoomPayload(DependencyObject depObj, ImageZoomPayload value) =>
            depObj.SetValue(ZoomPayloadProperty, value);

        #endregion

        #region ScrollOffsetCenterProperty

        private static readonly string ScrollOffsetCenter = nameof(ScrollOffsetCenter);

        private static readonly DependencyProperty ScrollOffsetCenterProperty =
            DependencyProperty.RegisterAttached(
                ScrollOffsetCenter,
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

        public static Size GetScrollOffsetCenter(DependencyObject depObj) =>
            (Size)depObj.GetValue(ScrollOffsetCenterProperty);

        public static void SetScrollOffsetCenter(DependencyObject depObj, Size value) =>
            depObj.SetValue(ScrollOffsetCenterProperty, value);

        #endregion


        public ImageScrollViewer()
        {
            InitializeComponent();

            #region EventHandlers

            MyScrollViewer.Loaded += (_, __) =>
            {
                // ScrollContentPresenterが生成されてから設定
                var scrollContentPresenter = GetChildControl<ScrollContentPresenter>(MyScrollViewer);
                scrollContentPresenter.PreviewMouseLeftButtonDown += (sender, e) => ScrollContentMouseLeftDown.Value = Unit.Default;
                scrollContentPresenter.PreviewMouseLeftButtonUp += (sender, e) => ScrollContentMouseLeftUp.Value = Unit.Default;
                scrollContentPresenter.MouseMove += (sender, e) => ScrollContentMouseMove.Value = e.GetPosition((IInputElement)sender);

                // 初期サイズはLoadedで取得しようとしたけどイベント来ないのでココで
                //scrollContentPresenter.Loaded += (sender, e) => ScrollContentActualSize.Value = new Size(scrollContentPresenter.Width, scrollContentPresenter.Height);
                ScrollContentActualSize.Value = new Size(scrollContentPresenter.ActualWidth, scrollContentPresenter.ActualHeight);
                scrollContentPresenter.SizeChanged += (sender, e) =>
                {
                    Debug.WriteLine($"***EventHandler_ScrollContent_SizeChanged: New={e.NewSize.Width} x {e.NewSize.Height}");
                    ScrollContentActualSize.Value = e.NewSize; //=ActualSize
                };
            };

            MyScrollViewer.PreviewMouseWheel += new MouseWheelEventHandler(MyScrollViewer_PreviewMouseWheel);
            MyScrollViewer.SizeChanged += (sender, e) =>
            {
                Debug.WriteLine($"***EventHandler_ScrollViewer_SizeChanged: New={e.NewSize.Width} x {e.NewSize.Height}");
            };
            MyScrollViewer.ScrollChanged += new ScrollChangedEventHandler(MyScrollViewer_ScrollChanged);

            MainImage.TargetUpdated += new EventHandler<DataTransferEventArgs>(MainImage_TargetUpdated);

            MainImage.SizeChanged += (sender, e) =>
            {
                Debug.WriteLine($"***EventHandler_Image_SizeChanged: New={e.NewSize.Width} x {e.NewSize.Height}");
                ImageViewActualSize.Value = e.NewSize; //=ActualSize
                MainImage_SizeChanged(sender, e);

                var payload = new ImageZoomPayload(ImageZoomMag.Value.IsEntire, GetCurrentZoomMagRatio());
                SetZoomPayload(MyScrollViewer, payload);
            };

            // ThumbCanvasのPreviewMouseWheelはMyScrollViewerに委託  ◆よりスマートな記述ありそう
            ThumbCanvas.PreviewMouseWheel += new MouseWheelEventHandler(MyScrollViewer_PreviewMouseWheel);

            //ThumbViewport.DragDelta += new DragDeltaEventHandler(OnDragDelta);

            #endregion

            // ダブルクリックイベントの自作 http://y-maeyama.hatenablog.com/entry/20110313/1300002095
            // ScrollViewerのMouseDoubleClickだとScrollBarのDoubleClickも拾ってしまうので
            // またScrollContentPresenterにMouseDoubleClickイベントは存在しない
            var preDoubleClick = ScrollContentMouseLeftDown
                .Select(_ => (Time: DateTime.Now, Point: ScrollContentMouseMove.Value))
                //.Do(x => Console.WriteLine($"SingleClick: {x.Time} {x.Point.X} x {x.Point.Y}"))
                .Pairwise()
                .Where(x => x.NewItem.Time.Subtract(x.OldItem.Time) <= TimeSpan.FromMilliseconds(500))
                // 高速に画像シフトするとダブルクリック判定されるのでマウス位置が動いていないことを見る
                .Where(x => Math.Abs(x.NewItem.Point.X - x.OldItem.Point.X) <= 3)
                .Where(x => Math.Abs(x.NewItem.Point.Y - x.OldItem.Point.Y) <= 3)
                .Select(_ => DateTime.Now)
                .ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);

            // ダブルクリックの2回が、3クリック目で2度目のダブルクリックになる対策
            // (ダブルクリック後に一定時間が経過するまでダブルクリックを採用しない)
            var ScrollContentDoubleClick = preDoubleClick
                .Pairwise()
                //.Do(x => Console.WriteLine($"PreDoubleClick2 {x.OldItem.ToString("HH:mm:ss.fff")}  {x.NewItem.ToString("HH:mm:ss.fff")}"))
                .Where(x => x.NewItem.Subtract(x.OldItem) >= TimeSpan.FromMilliseconds(200))
                .ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.None);
            
            ScrollContentDoubleClick.Subscribe(_ => SwitchClickZoomMag());

            // ズーム表示に切り替え
            ImageZoomMag
                .CombineLatest(ImageSourcePixelSize, (mag, imageSourceSize) => (mag, imageSourceSize))
                .Where(x => !x.mag.IsEntire)
                .Subscribe(x =>
                {
                    // ズーム表示からの全画面表示でスクロールバーが消えない対策
                    //   全画面表示中はスクロールバーを非表示にしているので、ズーム表示中は表示に戻す
                    MyScrollViewer.HorizontalScrollBarVisibility =
                    MyScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

                    MainImage.Width = x.imageSourceSize.Width * x.mag.MagnificationRatio;
                    MainImage.Height = x.imageSourceSize.Height * x.mag.MagnificationRatio;
                });

            // 全画面表示に切り替え
            ImageZoomMag
                .CombineLatest(ScrollContentActualSize, ImageSourcePixelSize, (mag, sview, imageSourceSize)
                    => (mag, sview, imageSourceSize))
                .Where(x => x.mag.IsEntire)
                .Subscribe(x =>
                {
                    // ズーム表示からの全画面表示でスクロールバーが消えない対策
                    //   スクロールバーが表示された状態で、親コントロール(ScrollViewer)とジャストのサイズに設定すると、
                    //   自動でスクロールバーが消えないので明示的に消す
                    MyScrollViewer.HorizontalScrollBarVisibility =
                    MyScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

                    var size = GetEntireZoomSize();
                    MainImage.Width = size.Width;
                    MainImage.Height = size.Height;
                });


            // 画像の現倍率計算
            //ImageViewActualSize
            //    .CombineLatest(ImageSourcePixelSize, (viewSize, sourceSize) => (viewSize, sourceSize))
            //    .Subscribe(x =>
            //    {
            //        var zoomRatio = Math.Min(x.viewSize.Width / x.sourceSize.Width, x.viewSize.Height / x.sourceSize.Height);
            //        Debug.WriteLine($"View_ImageZoomRatio: {zoomRatio * 100.0:f2} %");
            //    });

            // マウスホイールによるズーム倍率変更
            MouseWheelZoomDelta
                .CombineLatest(ImageSource, (delta, image) => (delta, image))
                .Where(x => x.delta != 0)
                .Subscribe(x =>
                {
                    var oldImageZoomMag = ImageZoomMag.Value;
                    var isZoomIn = x.delta > 0;

                    // ズーム前の倍率
                    double oldZoomMagRatio = GetCurrentZoomMagRatio();

                    // ズーム後のズーム管理クラス
                    var newImageZoomMag = oldImageZoomMag.ZoomMagnification(oldZoomMagRatio, isZoomIn);

                    // 全画面表示時を跨ぐ場合は全画面表示にする
                    var enrireZoomMag = GetEntireZoomMagRatio();
                    if ((oldImageZoomMag.MagnificationRatio < enrireZoomMag && enrireZoomMag < newImageZoomMag.MagnificationRatio)
                        || (newImageZoomMag.MagnificationRatio < enrireZoomMag && enrireZoomMag < oldImageZoomMag.MagnificationRatio))
                    {
                        ImageZoomMag.Value = new ImageZoomMagnification(true, enrireZoomMag);
                    }
                    else
                    {
                        ImageZoomMag.Value = newImageZoomMag;
                    }
                });

            // スクロールバーの移動
            ImageScrollOffsetRatio
                .CombineLatest(ScrollContentActualSize, ImageViewActualSize, ImageZoomMag,
                    (offset, sview, iview, _) => (offset, sview, iview, _))
                .Subscribe(x =>
                {
                    var width = Math.Max(0.0, x.offset.Width * x.iview.Width - (x.sview.Width / 2.0));
                    var height = Math.Max(0.0, x.offset.Height * x.iview.Height - (x.sview.Height / 2.0));

                    MyScrollViewer.ScrollToHorizontalOffset(width);
                    MyScrollViewer.ScrollToVerticalOffset(height);

                    // View→ViewModelへの通知
                    SetScrollOffsetCenter(MyScrollViewer, x.offset);
                });

            // ドラッグによる画像表示領域の移動
            //ScrollContentMouseMove
            //    .Pairwise()                                 // 最新値と前回値を取得
            //    .Select(x => -(x.NewItem - x.OldItem))      // 引っ張りと逆方向なので反転
            //    .SkipUntil(ScrollContentMouseLeftDown)
            //    .TakeUntil(ScrollContentMouseLeftUp)
            //    .Repeat()
            //    .Where(_ => !ImageZoomMag.Value.IsEntire)   // ズーム中のみ流す(全画面表示中は画像移動不要)
            //    //.Where(_ => !temporaryZoom.Value)           // ◆一時ズームは移動させない仕様
            //    .Subscribe(shift =>
            //    {
            //        double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            //        // マウスドラッグ中の表示位置のシフト
            //        double shiftRatioX = shift.X / ImageViewActualSize.Value.Width;
            //        double shiftRatioY = shift.Y / ImageViewActualSize.Value.Height;

            //        var extentWidth = ScrollChangedEventInfo.ExtentWidth;
            //        var extentHeight = ScrollChangedEventInfo.ExtentHeight;
            //        var viewportWidth = ScrollChangedEventInfo.ViewportWidth;
            //        var viewportHeight = ScrollChangedEventInfo.ViewportHeight;

            //        var widthRateMin = (viewportWidth / 2.0) / extentWidth;
            //        var heightRateMin = (viewportHeight / 2.0) / extentHeight;
            //        var widthRateMax = (extentWidth - viewportWidth / 2.0) / extentWidth;
            //        var heightRateMax = (extentHeight - viewportHeight / 2.0) / extentHeight;

            //        ImageScrollOffsetRatio.Value = new Size(
            //            clip(ImageScrollOffsetRatio.Value.Width + shiftRatioX, widthRateMin, widthRateMax),
            //            clip(ImageScrollOffsetRatio.Value.Height + shiftRatioY, heightRateMin, heightRateMax));
            //    });

        }

        // 全画面表示のサイズを取得
        private Size GetEntireZoomSize()
        {
            var imageRatio = ImageSourcePixelSize.Value.Width / ImageSourcePixelSize.Value.Height;
            var sview = ScrollContentActualSize.Value;

            double width, height;
            if (imageRatio > sview.Width / sview.Height)
            {
                width = sview.Width;      // 横パンパン
                height = sview.Width / imageRatio;
            }
            else
            {
                width = sview.Height * imageRatio;
                height = sview.Height;    // 縦パンパン
            }
            return new Size(width, height);
        }

        // 全画面表示のズーム倍率を取得
        private double GetEntireZoomMagRatio() => GetZoomMagRatio(GetEntireZoomSize());

        // 現在のズーム倍率を取得
        private double GetCurrentZoomMagRatio() => GetZoomMagRatio(ImageViewActualSize.Value);

        private double GetZoomMagRatio(Size size) =>
            Math.Min(size.Width / ImageSourcePixelSize.Value.Width,
                size.Height / ImageSourcePixelSize.Value.Height);

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

            // 全画面表示よりもズームしてるかフラグ(e.NewSize == Size of MainImage)
            // 小数点以下がちょいずれして意図通りの判定にならないことがあるので整数化する
            bool isZoomOverEntire = (Math.Floor(e.NewSize.Width) > Math.Floor(ScrollContentActualSize.Value.Width)
                || Math.Floor(e.NewSize.Height) > Math.Floor(ScrollContentActualSize.Value.Height));

            // ズーム倍率クラスの更新(TwoWay)
            //UpdateImageZoomMag(scrollViewer, image);

            // 全画面よりズームインしてたらサムネイル
            ThumbCanvas.Visibility = isZoomOverEntire ? Visibility.Visible : Visibility.Collapsed;

            // 全画面よりズームアウトしたらスクロールバー位置を初期化
            if (!isZoomOverEntire) ImageScrollOffsetRatio.Value = DefaultScrollOffsetRatio;

            UpdateBitmapScalingMode(image);
        }

        // ズーム倍率を設定(ViewModelに全画面表示の倍率を通知するため)
        //private static void UpdateImageZoomMag(ScrollViewer scrollViewer, Image image)
        //{
        //    if (!(image.Source is BitmapSource imageSource)) return;

        //    if (ImageZoomMag.Value.IsEntire)
        //    {
        //        var entireRatio = GetCurrentZoomMagnificationRatio(ImageZoomMag.Value, image, imageSource);
        //        ImageZoomMag.Value.SetsEntireMagnificationRatio(entireRatio);
        //    }
        //}

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
        //private static double GetCurrentZoomMagnificationRatio(ImageZoomMagnification imageZoomMag, Image image, BitmapSource imageSource)
        //{
        //    // 全画面表示でなければ倍率が入っているのでそのまま返す
        //    if (!imageZoomMag.IsEntire) return imageZoomMag.MagnificationRatio;

        //    return Math.Min(image.ActualWidth / imageSource.PixelWidth, image.ActualHeight / imageSource.PixelHeight);
        //}

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

            ScrollChangedEventInfo = e;

            if (ImageViewActualSize.Value.Width != 0.0 && ImageViewActualSize.Value.Height != 0.0)
            {
                var size = new Size((e.HorizontalOffset + e.ViewportWidth / 2.0) / ImageViewActualSize.Value.Width,
                    (e.VerticalOffset + e.ViewportHeight / 2.0) / ImageViewActualSize.Value.Height);

                // 全体表示なら中央に戻す
                if (e.ViewportWidth == e.ExtentWidth || e.ViewportHeight == e.ExtentHeight)
                {
                    size = DefaultScrollOffsetRatio;
                }

                //Debug.WriteLine($"ScrollChanged: {size.Width} x {size.Height}");
                ImageScrollOffsetRatio.Value = size;

                // View→ViewModel通知
                SetScrollOffsetCenter(MyScrollViewer, size);
            }
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
                // ここで倍率詰めるのは無理(コントロールサイズが変わっていないため)
                ImageZoomMag.Value = ImageZoomMagnification.Entire; // ToAll
            }

            // ズーム表示への切り替えならスクロールバーを移動(ImageViewSizeを変更した後に実施する)
            //if (!ImageZoomMag.Value.IsEntire)
            //{
            //    double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            //    var imageViewSize = new Size(MainImage.ActualWidth, MainImage.ActualHeight);
            //    var scrollViewerSize = ScrollViewerSize.Value;
            //    var imageSourceSize = new Size(ImageSource.Value.PixelWidth, ImageSource.Value.PixelHeight);
            //    var scrollVieweMousePoint = ScrollContentMouseMove.Value;

            //    // 親ScrollViewerから子Imageまでのサイズ
            //    var imageControlSizeOffset = new Size(
            //        Math.Max(0, scrollViewerSize.Width - imageViewSize.Width) / 2.0,
            //        Math.Max(0, scrollViewerSize.Height - imageViewSize.Height) / 2.0);

            //    // 子Image基準のマウス位置
            //    var mousePos = new Point(
            //        Math.Max(0, scrollVieweMousePoint.X - imageControlSizeOffset.Width),
            //        Math.Max(0, scrollVieweMousePoint.Y - imageControlSizeOffset.Height));

            //    // ズーム後の中心座標の割合
            //    var zoomCenterRate = new Size(
            //        clip(mousePos.X / imageViewSize.Width, 0.0, 1.0),
            //        clip(mousePos.Y / imageViewSize.Height, 0.0, 1.0));

            //    ImageScrollOffsetRatio.Value = zoomCenterRate;
            //}
        }


        private void MainImage_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            BitmapSource RoutedEventArgsToBiatmapSource(RoutedEventArgs e2)
            {
                if (!(e2.OriginalSource is Image image)) return null;
                if (!(image.Source is BitmapSource source)) return null;
                return source;
            }

            var imageSource = RoutedEventArgsToBiatmapSource(e);
            if (imageSource is null) return;

            // 初回画像は全画面表示
            bool isFirstTime = ImageSource.Value is null;

            ImageSource.Value = imageSource;
            ImageSourcePixelSize.Value = new Size(imageSource.PixelWidth, imageSource.PixelHeight);

            if (isFirstTime) ImageZoomMag.Value = ImageZoomMagnification.Entire;
        }

    }
}
