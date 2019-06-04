using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZoomThumb.ViewModels;
using ZoomThumb.Views.Common;

namespace ZoomThumb.Views.Controls
{
    // 尾上さんのお言葉
    // Viewに特別な要件が存在しない限りは、トリガーやアクションの自作にこだわらず積極的にコードビハインドを使いましょう
    // Viewのコードビハインドは、基本的にView内で完結するロジックとViewModelからのイベントの受信(専用リスナを使用する)に限るとトラブルが少なくなります

    /// <summary>
    /// ScrollImageViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class ScrollImageViewer : UserControl
    {
        // スクロールバー位置の初期値
        private static readonly Size DefaultScrollOffsetRatio = new Size(0.5, 0.5);

        // ズーム倍率(変数の操作用)
        internal ImageZoomMagnification ImageZoomMag
        {
            get => imageZoomMag.Value;
            set
            {
                if (!imageZoomMag.Value.Equals(value))
                    imageZoomMag.Value = value;
            }
        }

        // ズーム倍率(内部イベント用)
        private readonly ReactivePropertySlim<ImageZoomMagnification> imageZoomMag = new ReactivePropertySlim<ImageZoomMagnification>(ImageZoomMagnification.Entire);
        private readonly ReactivePropertySlim<Size> imageScrollOffsetRatio = new ReactivePropertySlim<Size>(DefaultScrollOffsetRatio);

        // スクロールバー除いた領域のコントロール（全画面でバーが消えた後にサイズ更新するために必要）
        private readonly ReactivePropertySlim<Size> scrollContentActualSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private readonly ReactivePropertySlim<Unit> scrollContentMouseLeftDown = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None);
        private readonly ReactivePropertySlim<Unit> scrollContentMouseLeftUp = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None);
        private readonly ReactivePropertySlim<Point> scrollContentMouseMove = new ReactivePropertySlim<Point>(mode: ReactivePropertyMode.DistinctUntilChanged);

        // 画像コントロール
        private readonly ReactivePropertySlim<Size> imageSourcePixelSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private readonly ReactivePropertySlim<Size> imageViewActualSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private readonly ReactivePropertySlim<int> mouseWheelZoomDelta = new ReactivePropertySlim<int>(mode: ReactivePropertyMode.None);

        #region ZoomPayloadProperty(TwoWay)

        // 画像ズーム倍率
        private static readonly string ZoomPayload = nameof(ZoomPayload);

        private static readonly DependencyProperty ZoomPayloadProperty =
            DependencyProperty.Register(
                nameof(ZoomPayload),
                typeof(ImageZoomPayload),
                typeof(ScrollImageViewer),
                new FrameworkPropertyMetadata(
                    default(ImageZoomPayload),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) =>
                    {
                        // ViewModel→View
                        if (e.NewValue is ImageZoomPayload payload)
                        {
                            var scrollImageViewer = ViewHelper.GetChildControl<ScrollImageViewer>(d);
                            if (scrollImageViewer != null)
                                scrollImageViewer.ImageZoomMag = new ImageZoomMagnification(payload);
                        }
                    }));

        public static ImageZoomPayload GetZoomPayload(DependencyObject depObj) =>
            (ImageZoomPayload)depObj.GetValue(ZoomPayloadProperty);

        public static void SetZoomPayload(DependencyObject depObj, ImageZoomPayload value) =>
            depObj.SetValue(ZoomPayloadProperty, value);

        #endregion

        #region ScrollOffsetCenterRatioProperty(TwoWay)

        // スクロールバーの位置割合(0~1)
        private static readonly string ScrollOffsetCenterRatio = nameof(ScrollOffsetCenterRatio);

        private static readonly DependencyProperty ScrollOffsetCenterRatioProperty =
            DependencyProperty.RegisterAttached(
                ScrollOffsetCenterRatio,
                typeof(Size),
                typeof(ScrollImageViewer),
                new FrameworkPropertyMetadata(
                    default(Size),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) =>
                    {
                        // ViewModel→View
                        if (e.NewValue is Size size)
                        {
                            var scrollImageViewer = ViewHelper.GetChildControl<ScrollImageViewer>(d);
                            if (scrollImageViewer != null)
                                scrollImageViewer.imageScrollOffsetRatio.Value = size;
                        }
                    }));

        public static Size GetScrollOffsetCenterRatio(DependencyObject depObj) =>
            (Size)depObj.GetValue(ScrollOffsetCenterRatioProperty);

        public static void SetScrollOffsetCenterRatio(DependencyObject depObj, Size value) =>
            depObj.SetValue(ScrollOffsetCenterRatioProperty, value);

        #endregion

        #region ScrollContentActualSizeProperty(OneWayToSource) ※いらんかも

        // スクロールバー除いた領域の実サイズ
        private static readonly string ScrollContentActualSize = nameof(ScrollContentActualSize);

        private static readonly DependencyProperty ScrollContentActualSizeProperty =
            DependencyProperty.Register(
                nameof(ScrollContentActualSize),
                typeof(Size),
                typeof(ScrollImageViewer),
                new FrameworkPropertyMetadata(
                    default(Size),
                    FrameworkPropertyMetadataOptions.None));

        public static Size GetScrollContentActualSize(DependencyObject depObj) =>
            (Size)depObj.GetValue(ScrollContentActualSizeProperty);

        public static void SetScrollContentActualSize(DependencyObject depObj, Size value) =>
            depObj.SetValue(ScrollContentActualSizeProperty, value);

        #endregion

        #region ImageViewActualSizeProperty(OneWayToSource) ※いらんかも

        // Imageコントロールの実サイズ
        private static readonly string ImageViewActualSize = nameof(ImageViewActualSize);

        private static readonly DependencyProperty ImageViewActualSizeProperty =
            DependencyProperty.Register(
                nameof(ImageViewActualSize),
                typeof(Size),
                typeof(ScrollImageViewer),
                new FrameworkPropertyMetadata(
                    default(Size),
                    FrameworkPropertyMetadataOptions.None));

        public static Size GetImageViewActualSize(DependencyObject depObj) =>
            (Size)depObj.GetValue(ImageViewActualSizeProperty);

        public static void SetImageViewActualSize(DependencyObject depObj, Size value) =>
            depObj.SetValue(ImageViewActualSizeProperty, value);

        #endregion

        #region ImageSourcePixelSizeProperty(OneWayToSource) ※いらんかも

        // Imageコントロールの実サイズ
        private static readonly string ImageSourcePixelSize = nameof(ImageSourcePixelSize);

        private static readonly DependencyProperty ImageSourcePixelSizeProperty =
            DependencyProperty.Register(
                nameof(ImageSourcePixelSize),
                typeof(Size),
                typeof(ScrollImageViewer),
                new FrameworkPropertyMetadata(
                    default(Size),
                    FrameworkPropertyMetadataOptions.None));

        public static Size GetImageSourcePixelSize(DependencyObject depObj) =>
            (Size)depObj.GetValue(ImageSourcePixelSizeProperty);

        public static void SetImageSourcePixelSize(DependencyObject depObj, Size value) =>
            depObj.SetValue(ImageSourcePixelSizeProperty, value);

        #endregion

        #region IsVisibleReducedImageProperty(OneWayToSource)

        // 縮小画像の表示切り替えフラグ(画像の全体表示中は非表示)
        private static readonly string IsVisibleReducedImage = nameof(IsVisibleReducedImage);

        private static readonly DependencyProperty IsVisibleReducedImageProperty =
            DependencyProperty.Register(
                nameof(IsVisibleReducedImage),
                typeof(bool),
                typeof(ScrollImageViewer),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.None));

        public static bool GetIsVisibleReducedImage(DependencyObject depObj) =>
            (bool)depObj.GetValue(IsVisibleReducedImageProperty);

        public static void SetIsVisibleReducedImage(DependencyObject depObj, bool value) =>
            depObj.SetValue(IsVisibleReducedImageProperty, value);

        #endregion

        #region ImageCursorPointProperty(OneWayToSource)

        // View画像上のカーソル位置(画像Pixel座標系)
        private static readonly string ImageCursorPoint = nameof(ImageCursorPoint);

        private static readonly DependencyProperty ImageCursorPointProperty =
            DependencyProperty.Register(
                nameof(ImageCursorPoint),
                typeof(Point),
                typeof(ScrollImageViewer),
                new FrameworkPropertyMetadata(
                    default(Point),
                    FrameworkPropertyMetadataOptions.None));

        public static Point GetImageCursorPoint(DependencyObject depObj) =>
            (Point)depObj.GetValue(ImageCursorPointProperty);

        public static void SetImageCursorPoint(DependencyObject depObj, Point value) =>
            depObj.SetValue(ImageCursorPointProperty, value);

        #endregion

        public ScrollImageViewer()
        {
            InitializeComponent();

            this.Loaded += (_, __) =>
            {
                var scrollContentPresenter = ViewHelper.GetChildControl<ScrollContentPresenter>(this);
                scrollContentPresenter.PreviewMouseLeftButtonDown += (sender, e) => scrollContentMouseLeftDown.Value = Unit.Default;
                scrollContentPresenter.PreviewMouseLeftButtonUp += (sender, e) => scrollContentMouseLeftUp.Value = Unit.Default;
                scrollContentPresenter.MouseMove += (sender, e) => scrollContentMouseMove.Value = e.GetPosition((IInputElement)sender);

                // 初期サイズはLoadedで取得しようとしたけどイベント来ないのでココで
                //scrollContentPresenter.Loaded += (sender, e) => 
                scrollContentActualSize.Value = ViewHelper.GetControlActualSize(scrollContentPresenter);
                scrollContentPresenter.SizeChanged += (sender, e) =>
                {
                    scrollContentActualSize.Value = e.NewSize; //=ActualSize
                };


                MainImage.TargetUpdated += (sender, e) =>
                {
                    if (!(e.OriginalSource is Image image)) return;
                    imageSourcePixelSize.Value = ViewHelper.GetImageSourcePixelSize(image);
                };
                MainImage.SizeChanged += (sender, e) =>
                {
                    imageViewActualSize.Value = e.NewSize; //=ActualSize
                    MainImage_SizeChanged(sender, e);
                };
                MainImage.MouseMove += (sender, e) =>
                {
                    double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

                    // 原画像のピクセル位置を返す
                    if (imageViewActualSize.Value.IsValidValue())
                    {
                        var cursorPoint = e.GetPosition((IInputElement)sender);
                        var x = Math.Round(cursorPoint.X * imageSourcePixelSize.Value.Width / imageViewActualSize.Value.Width);
                        x = clip(x, 0, imageSourcePixelSize.Value.Width - 1);
                        var y = Math.Round(cursorPoint.Y * imageSourcePixelSize.Value.Height / imageViewActualSize.Value.Height);
                        y = clip(y, 0, imageSourcePixelSize.Value.Height - 1);
                        SetImageCursorPoint(this, new Point(x, y));
                    }
                };


                if (VisualTreeHelper.GetParent(this) is Panel parentPanel)
                {
                    // サムネイルコントロール(Canvas)でもズーム操作を有効にするため、親パネルに添付イベントを貼る
                    parentPanel.AddHandler(PreviewMouseWheelEvent, new MouseWheelEventHandler(ScrollImageViewer_PreviewMouseWheel));
                }
            };

            MainScrollViewer.ScrollChanged += new ScrollChangedEventHandler(ScrollImageViewer_ScrollChanged);

            scrollContentActualSize.Subscribe(x => SetScrollContentActualSize(this, x));
            imageViewActualSize.Subscribe(x => SetImageViewActualSize(this, x));
            imageSourcePixelSize.Subscribe(x => SetImageSourcePixelSize(this, x));


            // ズーム倍率変更
            imageZoomMag.CombineLatest(imageSourcePixelSize, scrollContentActualSize,
                    (zoomMag, imageSourceSize, scrollContentSize) => (zoomMag, imageSourceSize, scrollContentSize))
                .Subscribe(x => UpdateImageZoom(x.zoomMag, x.imageSourceSize, x.scrollContentSize));

            #region DoubleClick

            // ダブルクリックイベントの自作 http://y-maeyama.hatenablog.com/entry/20110313/1300002095
            // ScrollViewerのMouseDoubleClickだとScrollBarのDoubleClickも拾ってしまうので
            // またScrollContentPresenterにMouseDoubleClickイベントは存在しない
            var preDoubleClick = scrollContentMouseLeftDown
                .Select(_ => (Time: DateTime.Now, Point: scrollContentMouseMove.Value))
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
            var scrollContentDoubleClick = preDoubleClick
                .Pairwise()
                //.Do(x => Console.WriteLine($"PreDoubleClick2 {x.OldItem.ToString("HH:mm:ss.fff")}  {x.NewItem.ToString("HH:mm:ss.fff")}"))
                .Where(x => x.NewItem.Subtract(x.OldItem) >= TimeSpan.FromMilliseconds(200))
                .ToUnit()
                .ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.None);

            scrollContentDoubleClick.Subscribe(_ => SwitchClickZoomMag());

            #endregion

            #region SingleClickZoom

            // 一時ズームフラグ
            var temporaryZoom = new ReactivePropertySlim<bool>(false, mode: ReactivePropertyMode.DistinctUntilChanged);

            // 長押しによる一時ズーム
            // Rx入門 (15) - スケジューラの利用 https://blog.xin9le.net/entry/2012/01/24/120722
            scrollContentMouseLeftDown
                .Throttle(TimeSpan.FromMilliseconds(300))       // 長押し判定
                .TakeUntil(scrollContentMouseLeftUp)            // 押下中のみ対象(ちょん離し後なら弾く)
                //.Do(x => Console.Write($"ThreadId: {Thread.CurrentThread.ManagedThreadId}"))
                .ObserveOnUIDispatcher()                        // 以降はUIスレッドに同期
                //.Do(x => Console.WriteLine($" -> {Thread.CurrentThread.ManagedThreadId}"))
                .Repeat()
                .Where(_ => ImageZoomMag.IsEntire)              // 全体表示なら流す(継続ズームを弾くため既にズームしてたら流さない)
                .Subscribe(_ => temporaryZoom.Value = true);

            // 一時ズーム解除
            scrollContentMouseLeftUp
                .Where(_ => temporaryZoom.Value)                // 一時ズームなら解除する(継続ズームは解除しない)
                .Subscribe(_ => temporaryZoom.Value = false);

            temporaryZoom.Subscribe(_ => SwitchClickZoomMag());

            #endregion

            #region MouseWheelZoom

            // マウスホイールによるズーム倍率変更
            mouseWheelZoomDelta
                .Where(x => x != 0)
                .Select(x => x > 0)
                .Subscribe(isZoomIn =>
                {
                    var oldImageZoomMag = ImageZoomMag;

                    // ズーム前の倍率
                    double oldZoomMagRatio = GetCurrentZoomMagRatio(imageViewActualSize.Value, imageSourcePixelSize.Value);

                    // ズーム後のズーム管理クラス
                    var newImageZoomMag = oldImageZoomMag.ZoomMagnification(oldZoomMagRatio, isZoomIn);

                    // 全画面表示時を跨ぐ場合は全画面表示にする
                    var enrireZoomMag = GetEntireZoomMagRatio(scrollContentActualSize.Value, imageSourcePixelSize.Value);

                    if ((oldImageZoomMag.MagnificationRatio < enrireZoomMag && enrireZoomMag < newImageZoomMag.MagnificationRatio)
                        || (newImageZoomMag.MagnificationRatio < enrireZoomMag && enrireZoomMag < oldImageZoomMag.MagnificationRatio))
                    {
                        ImageZoomMag = new ImageZoomMagnification(true, enrireZoomMag);
                    }
                    else
                    {
                        ImageZoomMag = newImageZoomMag;
                    }
                });

            #endregion

            #region ScrollOffset

            // スクロールバーの移動
            imageScrollOffsetRatio
                .CombineLatest(scrollContentActualSize, imageViewActualSize,
                    (offset, sview, iview) => (offset, sview, iview))
                .Subscribe(x =>
                {
                    double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

                    var scrollViewer = MainScrollViewer;

                    // 好き勝手に要求された位置を範囲制限する
                    var rateRange = GetScrollOffsetRateRange(scrollViewer);
                    var newOffset = new Size(
                        clip(x.offset.Width, rateRange.widthMin, rateRange.widthMax),
                        clip(x.offset.Height, rateRange.heightMin, rateRange.heightMax));

                    var sviewHalf = new Size(x.sview.Width / 2.0, x.sview.Height / 2.0);
                    if (!sviewHalf.IsValidValue()) return;

                    var width = Math.Max(0.0, newOffset.Width * x.iview.Width - sviewHalf.Width);
                    var height = Math.Max(0.0, newOffset.Height * x.iview.Height - sviewHalf.Height);

                    scrollViewer.ScrollToHorizontalOffset(width);
                    scrollViewer.ScrollToVerticalOffset(height);

                    // View→ViewModelへの通知
                    SetScrollOffsetCenterRatio(this, newOffset);
                });

            // ドラッグによる画像表示領域の移動
            scrollContentMouseMove
                .Pairwise()                                 // 最新値と前回値を取得
                .Select(x => -(x.NewItem - x.OldItem))      // 引っ張りと逆方向なので反転
                .SkipUntil(scrollContentMouseLeftDown)
                .TakeUntil(scrollContentMouseLeftUp)
                .Repeat()
                .Where(_ => !ImageZoomMag.IsEntire)         // ズーム中のみ流す(全画面表示中は画像移動不要)
                .Where(_ => !temporaryZoom.Value)           // ◆一時ズームは移動させない仕様
                .Subscribe(shift =>
                {
                    if (!imageViewActualSize.Value.IsValidValue()) return;

                    // マウスドラッグ中の表示位置のシフト
                    double shiftRatioX = shift.X / imageViewActualSize.Value.Width;
                    double shiftRatioY = shift.Y / imageViewActualSize.Value.Height;

                    imageScrollOffsetRatio.Value = new Size(
                        imageScrollOffsetRatio.Value.Width + shiftRatioX,
                        imageScrollOffsetRatio.Value.Height + shiftRatioY);
                });

            #endregion

        }

        #region ImageZoomMag

        private void UpdateImageZoom(ImageZoomMagnification zoomMagnification, Size imageSourceSize, Size scrollPresenterSize)
        {
            if (!imageSourceSize.IsValidValue()) return;

            var scrollViewer = MainScrollViewer;
            var image = MainImage;

            if (!zoomMagnification.IsEntire)
            {
                // ズーム表示に切り替え
                // 画像サイズの更新前にスクロールバーの表示を更新(ContentSizeに影響出るので)
                UpdateScrollBarVisibility(scrollViewer, zoomMagnification, scrollPresenterSize, imageSourceSize);

                image.Width = imageSourceSize.Width * zoomMagnification.MagnificationRatio;
                image.Height = imageSourceSize.Height * zoomMagnification.MagnificationRatio;
            }
            else
            {
                // 全画面表示に切り替え
                // 画像サイズの更新前にスクロールバーの表示を更新(ContentSizeに影響出るので)
                UpdateScrollBarVisibility(scrollViewer, zoomMagnification, scrollPresenterSize, imageSourceSize);

                var size = GetEntireZoomSize(scrollPresenterSize, imageSourceSize);
                image.Width = size.Width;
                image.Height = size.Height;
            }
        }

        private static void UpdateScrollBarVisibility(ScrollViewer scrollViewer, ImageZoomMagnification zoomMag, Size sviewSize, Size sourceSize)
        {
            var visible = ScrollBarVisibility.Hidden;

            // ズームインならスクロールバーを表示
            if (!zoomMag.IsEntire && (GetEntireZoomMagRatio(sviewSize, sourceSize) < zoomMag.MagnificationRatio))
                visible = ScrollBarVisibility.Visible;

            scrollViewer.HorizontalScrollBarVisibility =
            scrollViewer.VerticalScrollBarVisibility = visible;
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

        #region MouseWheelZoom

        private void ScrollImageViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                mouseWheelZoomDelta.Value = e.Delta;

                // 最大ズームでホイールすると画像の表示エリアが移動しちゃうので止める
                e.Handled = true;
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
            bool isZoomOverEntire = (Math.Floor(imageViewActualSize.Width) > Math.Floor(scrollContentActualSize.Value.Width)
                || Math.Floor(imageViewActualSize.Height) > Math.Floor(scrollContentActualSize.Value.Height));

            // 全画面よりズームインしてたら縮小画像を表示
            bool reducedImageVisible = isZoomOverEntire;
            SetIsVisibleReducedImage(this, reducedImageVisible);

            // 全画面よりズームアウトしたらスクロールバー位置を初期化
            if (!isZoomOverEntire) imageScrollOffsetRatio.Value = DefaultScrollOffsetRatio;

            // View→ViewModel
            var magRatio = GetCurrentZoomMagRatio(imageViewActualSize, imageSourcePixelSize.Value);
            var payload = new ImageZoomPayload(ImageZoomMag.IsEntire, magRatio);
            SetZoomPayload(this, payload);
        }

        #endregion

        #region SwitchClickZoomMag

        // クリックズームの状態を切り替える(全画面⇔ズーム)
        private void SwitchClickZoomMag()
        {
            if (!ImageZoomMag.IsEntire)
            {
                // ここで倍率詰めるのは無理(コントロールサイズが変わっていないため)
                ImageZoomMag = ImageZoomMagnification.Entire; // ToAll
            }
            else
            {
                ImageZoomMag = ImageZoomMagnification.MagX1;  // ToZoom

                // ズーム表示への切り替えならスクロールバーを移動(ImageViewSizeを変更した後に実施する)
                {
                    double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

                    // 親ScrollViewerから子Imageまでのサイズ
                    var imageControlSizeOffset = new Size(
                        Math.Max(0.0, scrollContentActualSize.Value.Width - imageViewActualSize.Value.Width) / 2.0,
                        Math.Max(0.0, scrollContentActualSize.Value.Height - imageViewActualSize.Value.Height) / 2.0);

                    // 子Image基準のマウス位置
                    var mousePos = new Point(
                        Math.Max(0.0, scrollContentMouseMove.Value.X - imageControlSizeOffset.Width),
                        Math.Max(0.0, scrollContentMouseMove.Value.Y - imageControlSizeOffset.Height));

                    // ズーム後の中心座標の割合
                    var zoomCenterRate = new Size(
                        clip(mousePos.X / imageViewActualSize.Value.Width, 0.0, 1.0),
                        clip(mousePos.Y / imageViewActualSize.Value.Height, 0.0, 1.0));

                    imageScrollOffsetRatio.Value = zoomCenterRate;
                }
            }
        }

        #endregion

        #region ScrollChanged

        private void ScrollImageViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (imageViewActualSize.Value.IsValidValue())
            {
                var size = new Size(
                    (e.HorizontalOffset + e.ViewportWidth / 2.0) / imageViewActualSize.Value.Width,
                    (e.VerticalOffset + e.ViewportHeight / 2.0) / imageViewActualSize.Value.Height);

                // 全体表示なら中央位置を上書き
                if (e.ViewportWidth == e.ExtentWidth || e.ViewportHeight == e.ExtentHeight)
                    size = DefaultScrollOffsetRatio;

                //Debug.WriteLine($"ScrollChanged: {size.Width} x {size.Height}");
                imageScrollOffsetRatio.Value = size;

                // View→ViewModel通知
                SetScrollOffsetCenterRatio(this, size);
            }
        }

        // スクロールバー位置の範囲(割合)を取得
        private static (double widthMin, double widthMax, double heightMin, double heightMax)
            GetScrollOffsetRateRange(ScrollViewer sView)
        {
            (double, double, double, double) nolimit = (0.0, 1.0, 0.0, 1.0);

            // 全体表示ならオフセットに制限なし
            if (sView.ExtentWidth < sView.ViewportWidth || sView.ExtentHeight < sView.ViewportHeight)
            {
                return nolimit;
            }
            else if (sView.ExtentWidth.IsValidValue() && sView.ExtentHeight.IsValidValue())
            {
                var widthRateMin = (sView.ViewportWidth / 2.0) / sView.ExtentWidth;
                var widthRateMax = (sView.ExtentWidth - sView.ViewportWidth / 2.0) / sView.ExtentWidth;
                var heightRateMin = (sView.ViewportHeight / 2.0) / sView.ExtentHeight;
                var heightRateMax = (sView.ExtentHeight - sView.ViewportHeight / 2.0) / sView.ExtentHeight;
                return (widthRateMin, widthRateMax, heightRateMin, heightRateMax);
            }
            return nolimit;
        }

        #endregion

    }
}
