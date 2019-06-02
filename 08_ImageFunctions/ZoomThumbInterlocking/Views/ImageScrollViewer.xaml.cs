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
using ZoomThumb.Views.Common;

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

        // 各コントロールの連動フィールド
        private static readonly ReactivePropertySlim<Size> ImageScrollOffsetRatioShare = new ReactivePropertySlim<Size>(DefaultScrollOffsetRatio, mode: ReactivePropertyMode.None);

        // 自コントロールの状態
        private static readonly ReactivePropertySlim<ImageZoomMagnification> ImageZoomMag = new ReactivePropertySlim<ImageZoomMagnification>(ImageZoomMagnification.Entire);
        private readonly ReactivePropertySlim<Size> ImageScrollOffsetRatio = new ReactivePropertySlim<Size>(DefaultScrollOffsetRatio);

        //private readonly ReactivePropertySlim<BitmapSource> ImageSource = new ReactivePropertySlim<BitmapSource>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private readonly ReactivePropertySlim<Size> ImageSourcePixelSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private readonly ReactivePropertySlim<Size> ImageViewActualSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private readonly ReactivePropertySlim<int> MouseWheelZoomDelta = new ReactivePropertySlim<int>(mode: ReactivePropertyMode.None);

        private readonly ReactivePropertySlim<Size> ScrollContentActualSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private readonly ReactivePropertySlim<Unit> ScrollContentMouseLeftDown = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None);
        private readonly ReactivePropertySlim<Unit> ScrollContentMouseLeftUp = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None);
        private readonly ReactivePropertySlim<Point> ScrollContentMouseMove = new ReactivePropertySlim<Point>(mode: ReactivePropertyMode.None);
        //private readonly ReactivePropertySlim<Unit> ScrollContentDoubleClick = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None);

        private (double widthMin, double widthMax, double heightMin, double heightMax) ScrollOffsetRateRange;

        #region ZoomPayloadProperty

        private static readonly string ZoomPayload = nameof(ZoomPayload);

        private static readonly DependencyProperty ZoomPayloadProperty =
            DependencyProperty.RegisterAttached(
                nameof(ZoomPayload),
                typeof(ImageZoomPayload),
                typeof(ImageScrollViewer),
                new FrameworkPropertyMetadata(
                    default(ImageZoomPayload),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) =>
                    {
                        // ViewModel→View
                        if (e.NewValue is ImageZoomPayload payload)
                            ImageZoomMag.Value = new ImageZoomMagnification(payload);
                    }));

        public static ImageZoomPayload GetZoomPayload(DependencyObject depObj) =>
            (ImageZoomPayload)depObj.GetValue(ZoomPayloadProperty);

        public static void SetZoomPayload(DependencyObject depObj, ImageZoomPayload value) =>
            depObj.SetValue(ZoomPayloadProperty, value);

        #endregion

        #region ScrollOffsetCenterRatioProperty

        private static readonly string ScrollOffsetCenterRatio = nameof(ScrollOffsetCenterRatio);

        private static readonly DependencyProperty ScrollOffsetCenterRatioProperty =
            DependencyProperty.RegisterAttached(
                ScrollOffsetCenterRatio,
                typeof(Size),
                typeof(ImageScrollViewer),
                new FrameworkPropertyMetadata(
                    default(Size),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) =>
                    {
                        // ViewModel→View
                        if (!(d is ScrollViewer scrollViewer)) return;
                        if (e.NewValue is Size newRatio)
                        {
                            if (GetIsImageViewerInterlock(scrollViewer))
                            {
                                ImageScrollOffsetRatioShare.Value = newRatio;
                            }
                            else
                            {
                                // Viewからのスクロール位置割合の通知を基にコントロールを操作する
                                var scrollContentPresenter = ViewHelper.GetChildControl<ScrollContentPresenter>(scrollViewer);
                                var imageView = ViewHelper.GetChildControl<Image>(scrollViewer);

                                var newHOffset = Math.Max(0.0, newRatio.Width * imageView.ActualWidth - (scrollContentPresenter.ActualWidth / 2.0));
                                var newVOffset = Math.Max(0.0, newRatio.Height * imageView.ActualHeight - (scrollContentPresenter.ActualHeight / 2.0));

                                scrollViewer.ScrollToHorizontalOffset(newHOffset);
                                scrollViewer.ScrollToVerticalOffset(newVOffset);
                            }
                        }
                    }));

        public static Size GetScrollOffsetCenterRatio(DependencyObject depObj) =>
            (Size)depObj.GetValue(ScrollOffsetCenterRatioProperty);

        public static void SetScrollOffsetCenterRatio(DependencyObject depObj, Size value) =>
            depObj.SetValue(ScrollOffsetCenterRatioProperty, value);

        #endregion

        #region IsImageViewerInterlockProperty

        private static readonly string IsImageViewerInterlock = nameof(IsImageViewerInterlock);

        private static readonly DependencyProperty IsImageViewerInterlockProperty =
            DependencyProperty.RegisterAttached(
                IsImageViewerInterlock,
                typeof(bool),
                typeof(ImageScrollViewer));

        public static bool GetIsImageViewerInterlock(DependencyObject depObj) =>
            (bool)depObj.GetValue(IsImageViewerInterlockProperty);

        public static void SetIsImageViewerInterlock(DependencyObject depObj, bool value) =>
            depObj.SetValue(IsImageViewerInterlockProperty, value);

        #endregion

        public ImageScrollViewer()
        {
            InitializeComponent();

            #region EventHandlers

            MainScrollViewer.Loaded += (_, __) =>
            {
                // ScrollContentPresenterが生成されてから設定
                var scrollContentPresenter = ViewHelper.GetChildControl<ScrollContentPresenter>(MainScrollViewer);
                scrollContentPresenter.PreviewMouseLeftButtonDown += (sender, e) => ScrollContentMouseLeftDown.Value = Unit.Default;
                scrollContentPresenter.PreviewMouseLeftButtonUp += (sender, e) => ScrollContentMouseLeftUp.Value = Unit.Default;
                scrollContentPresenter.MouseMove += (sender, e) => ScrollContentMouseMove.Value = e.GetPosition((IInputElement)sender);

                // 初期サイズはLoadedで取得しようとしたけどイベント来ないのでココで
                //scrollContentPresenter.Loaded += (sender, e) => ScrollContentActualSize.Value = new Size(scrollContentPresenter.Width, scrollContentPresenter.Height);
                ScrollContentActualSize.Value = new Size(scrollContentPresenter.ActualWidth, scrollContentPresenter.ActualHeight);
                scrollContentPresenter.SizeChanged += (sender, e) =>
                {
                    //Debug.WriteLine($"***EventHandler_ScrollContent_SizeChanged: New={e.NewSize.Width} x {e.NewSize.Height}");
                    ScrollContentActualSize.Value = e.NewSize; //=ActualSize
                };
            };

            MainScrollViewer.PreviewMouseWheel += new MouseWheelEventHandler(MainScrollViewer_PreviewMouseWheel);
            //MainScrollViewer.SizeChanged += (sender, e) => Debug.WriteLine($"***EventHandler_ScrollViewer_SizeChanged: New={e.NewSize.Width} x {e.NewSize.Height}");
            MainScrollViewer.ScrollChanged += new ScrollChangedEventHandler(MainScrollViewer_ScrollChanged);

            MainImage.TargetUpdated += (sender, e) =>
            {
                if (!(e.OriginalSource is Image image)) return;
                if (!(image.Source is BitmapSource source)) return;
                ImageSourcePixelSize.Value = new Size(source.PixelWidth, source.PixelHeight);
            };
            MainImage.SizeChanged += (sender, e) =>
            {
                //Debug.WriteLine($"***EventHandler_Image_SizeChanged: New={e.NewSize.Width} x {e.NewSize.Height}");
                ImageViewActualSize.Value = e.NewSize; //=ActualSize
                MainImage_SizeChanged(sender, e);
            };

            // ThumbCanvasのPreviewMouseWheelはMainScrollViewerに委託  ◆よりスマートな記述ありそう
            ThumbCanvas.PreviewMouseWheel += new MouseWheelEventHandler(MainScrollViewer_PreviewMouseWheel);

            ThumbViewport.DragDelta += new DragDeltaEventHandler(OnDragDelta);

            #endregion

            #region DoubleClick

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
                .ToUnit()
                .ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.None);
            
            ScrollContentDoubleClick.Subscribe(_ => SwitchClickZoomMag());

            #endregion

            #region SingleClickZoom

            // 一時ズームフラグ
            var temporaryZoom = new ReactivePropertySlim<bool>(false, mode: ReactivePropertyMode.DistinctUntilChanged);

            // 長押しによる一時ズーム
            // Rx入門 (15) - スケジューラの利用 https://blog.xin9le.net/entry/2012/01/24/120722
            ScrollContentMouseLeftDown
                .Throttle(TimeSpan.FromMilliseconds(300))       // 長押し判定
                .TakeUntil(ScrollContentMouseLeftUp)            // 押下中のみ対象(ちょん離し後なら弾く)
                //.Do(x => Console.Write($"ThreadId: {Thread.CurrentThread.ManagedThreadId}"))
                .ObserveOnUIDispatcher()                        // 以降はUIスレッドに同期
                //.Do(x => Console.WriteLine($" -> {Thread.CurrentThread.ManagedThreadId}"))
                .Repeat()
                .Where(_ => ImageZoomMag.Value.IsEntire)        // 全体表示なら流す(継続ズームを弾くため既にズームしてたら流さない)
                .Subscribe(_ => temporaryZoom.Value = true);

            // 一時ズーム解除
            ScrollContentMouseLeftUp
                .Where(_ => temporaryZoom.Value)                // 一時ズームなら解除する(継続ズームは解除しない)
                .Subscribe(_ => temporaryZoom.Value = false);

            temporaryZoom.Subscribe(_ => SwitchClickZoomMag());

            #endregion

            #region ImageZoomMag

            // ズーム表示に切り替え
            ImageZoomMag
                .CombineLatest(ImageSourcePixelSize, (mag, imageSourceSize) => (mag, imageSourceSize))
                .Where(x => !x.mag.IsEntire)
                .Subscribe(x =>
                {
                    // 画像サイズの更新前にスクロールバーの表示を更新(ContentSizeに影響出るので)
                    UpdateScrollBarVisibility(x.mag, ScrollContentActualSize.Value, x.imageSourceSize);

                    MainImage.Width = x.imageSourceSize.Width * x.mag.MagnificationRatio;
                    MainImage.Height = x.imageSourceSize.Height * x.mag.MagnificationRatio;
                });

            // 全画面表示に切り替え
            ImageZoomMag
                .CombineLatest(ScrollContentActualSize, ImageSourcePixelSize, (mag, sviewSize, sourceSize)
                    => (mag, sviewSize, sourceSize))
                .Where(x => x.mag.IsEntire)
                .Subscribe(x =>
                {
                    // 画像サイズの更新前にスクロールバーの表示を更新(ContentSizeに影響出るので)
                    UpdateScrollBarVisibility(x.mag, x.sviewSize, x.sourceSize);

                    var size = GetEntireZoomSize(x.sviewSize, x.sourceSize);
                    MainImage.Width = size.Width;
                    MainImage.Height = size.Height;
                });

            #endregion

            #region MouseWheel

            // マウスホイールによるズーム倍率変更
            MouseWheelZoomDelta
                .Where(x => x != 0)
                .Select(x => x > 0)
                .Subscribe(isZoomIn =>
                {
                    var oldImageZoomMag = ImageZoomMag.Value;

                    // ズーム前の倍率
                    double oldZoomMagRatio = GetCurrentZoomMagRatio(ImageViewActualSize.Value, ImageSourcePixelSize.Value);

                    // ズーム後のズーム管理クラス
                    var newImageZoomMag = oldImageZoomMag.ZoomMagnification(oldZoomMagRatio, isZoomIn);

                    // 全画面表示時を跨ぐ場合は全画面表示にする
                    var enrireZoomMag = GetEntireZoomMagRatio(ScrollContentActualSize.Value, ImageSourcePixelSize.Value);
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

            #endregion

            #region ScrollOffset

            // スクロールバー移動の連動(自コントロールに移譲)
            ImageScrollOffsetRatioShare.Subscribe(x => ImageScrollOffsetRatio.Value = x);

            // スクロールバーの移動
            ImageScrollOffsetRatio
                .CombineLatest(ScrollContentActualSize, ImageViewActualSize, ImageZoomMag,
            (offset, sview, iview, _) => (offset, sview, iview, _))
                .Subscribe(x =>
                {
                    var width = Math.Max(0.0, x.offset.Width * x.iview.Width - (x.sview.Width / 2.0));
                    var height = Math.Max(0.0, x.offset.Height * x.iview.Height - (x.sview.Height / 2.0));

                    MainScrollViewer.ScrollToHorizontalOffset(width);
                    MainScrollViewer.ScrollToVerticalOffset(height);

                    // View→ViewModelへの通知
                    SetScrollOffsetCenterRatio(MainScrollViewer, x.offset);
                });

            // ドラッグによる画像表示領域の移動
            ScrollContentMouseMove
                .Pairwise()                                 // 最新値と前回値を取得
                .Select(x => -(x.NewItem - x.OldItem))      // 引っ張りと逆方向なので反転
                .SkipUntil(ScrollContentMouseLeftDown)
                .TakeUntil(ScrollContentMouseLeftUp)
                .Repeat()
                .Where(_ => !ImageZoomMag.Value.IsEntire)   // ズーム中のみ流す(全画面表示中は画像移動不要)
                .Where(_ => !temporaryZoom.Value)           // ◆一時ズームは移動させない仕様
                .Subscribe(shift =>
                {
                    double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

                    // マウスドラッグ中の表示位置のシフト
                    double shiftRatioX = shift.X / ImageViewActualSize.Value.Width;
                    double shiftRatioY = shift.Y / ImageViewActualSize.Value.Height;

                    var (widthMin, widthMax, heightMin, heightMax) = ScrollOffsetRateRange;

                    ImageScrollOffsetRatio.Value = new Size(
                        clip(ImageScrollOffsetRatio.Value.Width + shiftRatioX, widthMin, widthMax),
                        clip(ImageScrollOffsetRatio.Value.Height + shiftRatioY, heightMin, heightMax));
                });

            #endregion

        }

        #region ImageZoomMag

        private void UpdateScrollBarVisibility(ImageZoomMagnification zoomMag, Size sviewSize, Size sourceSize)
        {
            var visible = ScrollBarVisibility.Hidden;

            // ズームインならスクロールバーを表示
            if (!zoomMag.IsEntire && (GetEntireZoomMagRatio(sviewSize, sourceSize) < zoomMag.MagnificationRatio))
                visible = ScrollBarVisibility.Visible;

            MainScrollViewer.HorizontalScrollBarVisibility =
            MainScrollViewer.VerticalScrollBarVisibility = visible;
        }

        #endregion

        #region ZoomSize

        // 全画面表示のサイズを取得
        private static Size GetEntireZoomSize(Size sviewSize, Size sourceSize)
        {
            var imageRatio = sourceSize.Width / sourceSize.Height;

            double width, height;
            if (imageRatio > sviewSize.Width / sviewSize.Height)
            {
                width = sviewSize.Width;      // 横パンパン
                height = sviewSize.Width / imageRatio;
            }
            else
            {
                width = sviewSize.Height * imageRatio;
                height = sviewSize.Height;    // 縦パンパン
            }
            return new Size(width, height);
        }

        // 全画面表示のズーム倍率を取得
        private static double GetEntireZoomMagRatio(Size sviewSize, Size sourceSize) =>
            GetZoomMagRatio(GetEntireZoomSize(sviewSize, sourceSize), sourceSize);
            
        // 現在のズーム倍率を取得
        private static double GetCurrentZoomMagRatio(Size imageViewSize, Size imageSourceSize) =>
            GetZoomMagRatio(imageViewSize, imageSourceSize);

        // 引数サイズのズーム倍率を求める
        private static double GetZoomMagRatio(Size newSize, Size baseSize) =>
            Math.Min(newSize.Width / baseSize.Width, newSize.Height / baseSize.Height);

        #endregion

        #region SwitchClickZoomMag

        // クリックズームの状態を切り替える(全画面⇔ズーム)
        private void SwitchClickZoomMag()
        {
            if (!ImageZoomMag.Value.IsEntire)
            {
                // ここで倍率詰めるのは無理(コントロールサイズが変わっていないため)
                ImageZoomMag.Value = ImageZoomMagnification.Entire; // ToAll
            }
            else
            {
                ImageZoomMag.Value = ImageZoomMagnification.MagX1;  // ToZoom

                // ズーム表示への切り替えならスクロールバーを移動(ImageViewSizeを変更した後に実施する)
                {
                    double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

                    // 親ScrollViewerから子Imageまでのサイズ
                    var imageControlSizeOffset = new Size(
                        Math.Max(0.0, ScrollContentActualSize.Value.Width - ImageViewActualSize.Value.Width) / 2.0,
                        Math.Max(0.0, ScrollContentActualSize.Value.Height - ImageViewActualSize.Value.Height) / 2.0);

                    // 子Image基準のマウス位置
                    var mousePos = new Point(
                        Math.Max(0.0, ScrollContentMouseMove.Value.X - imageControlSizeOffset.Width),
                        Math.Max(0.0, ScrollContentMouseMove.Value.Y - imageControlSizeOffset.Height));

                    // ズーム後の中心座標の割合
                    var zoomCenterRate = new Size(
                        clip(mousePos.X / ImageViewActualSize.Value.Width, 0.0, 1.0),
                        clip(mousePos.Y / ImageViewActualSize.Value.Height, 0.0, 1.0));

                    ImageScrollOffsetRatio.Value = zoomCenterRate;
                }
            }
        }

        #endregion

        #region ImageSizeChanged

        // 画像のサイズ変更(ズーム操作)
        private void MainImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!(e.OriginalSource is Image image)) return;

            var imageViewActualSize = e.NewSize;

            // 全画面表示よりもズームしてるかフラグ(e.NewSize == Size of MainImage)
            // 小数点以下がちょいずれして意図通りの判定にならないことがあるので整数化する
            bool isZoomOverEntire = (Math.Floor(imageViewActualSize.Width) > Math.Floor(ScrollContentActualSize.Value.Width)
                || Math.Floor(imageViewActualSize.Height) > Math.Floor(ScrollContentActualSize.Value.Height));

            // 全画面よりズームインしてたらサムネイル
            ThumbCanvas.Visibility = isZoomOverEntire ? Visibility.Visible : Visibility.Collapsed;

            // 全画面よりズームアウトしたらスクロールバー位置を初期化
            if (!isZoomOverEntire) ImageScrollOffsetRatio.Value = DefaultScrollOffsetRatio;

            UpdateBitmapScalingMode(image);

            // View→ViewModel
            var magRatio = GetCurrentZoomMagRatio(imageViewActualSize, ImageSourcePixelSize.Value);
            var payload = new ImageZoomPayload(ImageZoomMag.Value.IsEntire, magRatio);
            SetZoomPayload(MainScrollViewer, payload);
        }

        // レンダリングオプションの指定(100%以上の拡大ズームならPixelが見える設定にする)
        private void UpdateBitmapScalingMode(Image image)
        {
            if (!(image.Source is BitmapSource bitmap)) return;

            var mode = (image.ActualWidth < bitmap.PixelWidth) || (image.ActualHeight < bitmap.PixelHeight)
                ? BitmapScalingMode.HighQuality : BitmapScalingMode.NearestNeighbor;

            RenderOptions.SetBitmapScalingMode(image, mode);
        }

        #endregion

        #region MouseWheelZoom

        private void MainScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
        
        #endregion

        #region ThumbnailViewport

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);
            var (widthMin, widthMax, heightMin, heightMax) = ScrollOffsetRateRange;

            var width = MainScrollViewer.HorizontalOffset + (e.HorizontalChange * MainScrollViewer.ExtentWidth / Thumbnail.ActualWidth);
            width += MainScrollViewer.ActualWidth / 2.0;
            width /= ImageViewActualSize.Value.Width;
            width = clip(width, widthMin, widthMax);

            var height = MainScrollViewer.VerticalOffset + (e.VerticalChange * MainScrollViewer.ExtentHeight / Thumbnail.ActualHeight);
            height += MainScrollViewer.ActualHeight / 2.0;
            height /= ImageViewActualSize.Value.Height;
            height = clip(height, heightMin, heightMax);

            // スクロール位置を更新
            ImageScrollOffsetRatio.Value = new Size(width, height);
        }

        private void MainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UpdateThumbnailViewport(sender, e);

            // スクロールの範囲(割合)を更新
            UpdateScrollOffsetRateRange(e);

            if (ImageViewActualSize.Value.Width != 0.0 && ImageViewActualSize.Value.Height != 0.0)
            {
                var size = new Size(
                    (e.HorizontalOffset + e.ViewportWidth / 2.0) / ImageViewActualSize.Value.Width,
                    (e.VerticalOffset + e.ViewportHeight / 2.0) / ImageViewActualSize.Value.Height);

                // 全体表示なら中央位置を上書き
                if (e.ViewportWidth == e.ExtentWidth || e.ViewportHeight == e.ExtentHeight)
                    size = DefaultScrollOffsetRatio;

                //Debug.WriteLine($"ScrollChanged: {size.Width} x {size.Height}");
                ImageScrollOffsetRatio.Value = size;

                // View→ViewModel通知
                SetScrollOffsetCenterRatio(MainScrollViewer, size);
            }
        }

        private void UpdateScrollOffsetRateRange(ScrollChangedEventArgs e)
        {
            // 全体表示ならオフセットに制限なし
            if (e.ExtentWidth < e.ViewportWidth || e.ExtentHeight < e.ViewportHeight)
            {
                ScrollOffsetRateRange = (0.0, 1.0, 0.0, 1.0);
            }
            else if (e.ExtentWidth != 0.0 && e.ExtentHeight != 0.0)
            {
                var widthRateMin = (e.ViewportWidth / 2.0) / e.ExtentWidth;
                var widthRateMax = (e.ExtentWidth - e.ViewportWidth / 2.0) / e.ExtentWidth;
                var heightRateMin = (e.ViewportHeight / 2.0) / e.ExtentHeight;
                var heightRateMax = (e.ExtentHeight - e.ViewportHeight / 2.0) / e.ExtentHeight;
                ScrollOffsetRateRange = (widthRateMin, widthRateMax, heightRateMin, heightRateMax);
            }
        }

        private void UpdateThumbnailViewport(object sender, ScrollChangedEventArgs e)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            var thumbnail = Thumbnail;
            var thumbViewport = ThumbViewport;

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

            CombinedGeometry.Geometry2 = new RectangleGeometry(new Rect(left, top, width, height));
        }

        #endregion

    }
}
