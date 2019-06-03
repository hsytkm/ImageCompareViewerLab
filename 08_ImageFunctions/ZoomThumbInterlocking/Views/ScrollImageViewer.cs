using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZoomThumb.Views.Common;

namespace ZoomThumb.Views
{
    class ScrollImageViewer : ScrollViewer
    {
        // 内部コントロールの参照
        public ScrollContentPresenter InsideScrollPresenter { get; private set; }
        public Image InsideImage { get; private set; }

        // ズーム倍率(変数の操作用)
        public ImageZoomMagnification ImageZoomMagni
        {
            get => _ImageZoomMagni.Value;
            set
            {
                if (!_ImageZoomMagni.Value.Equals(value))
                    _ImageZoomMagni.Value = value;
            }
        }

        // ズーム倍率(内部イベント用)
        private readonly ReactivePropertySlim<ImageZoomMagnification> _ImageZoomMagni =
            new ReactivePropertySlim<ImageZoomMagnification>(ImageZoomMagnification.Entire);

        // スクロールバー除いた領域のコントロール（全画面でバーが消えた後にサイズ更新するために必要）
        private readonly ReactivePropertySlim<Size> ScrollContentActualSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);

        // 画像コントロール
        private readonly ReactivePropertySlim<Size> ImageSourcePixelSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private readonly ReactivePropertySlim<Size> ImageViewActualSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);
        private readonly ReactivePropertySlim<int> MouseWheelZoomDelta = new ReactivePropertySlim<int>(mode: ReactivePropertyMode.None);

        #region VisibilityReducedImageProperty

        // ズーム時の縮小画像表示フラグ
        private static readonly string VisibilityReducedImage = nameof(VisibilityReducedImage);

        private static readonly DependencyProperty VisibilityReducedImageProperty =
            DependencyProperty.Register(
                nameof(VisibilityReducedImage),
                typeof(Visibility),
                typeof(ScrollImageViewer),
                new FrameworkPropertyMetadata(
                    Visibility.Collapsed,
                    FrameworkPropertyMetadataOptions.None));

        public static Visibility GetVisibilityReducedImage(DependencyObject depObj) =>
            (Visibility)depObj.GetValue(VisibilityReducedImageProperty);

        public static void SetVisibilityReducedImage(DependencyObject depObj, Visibility value) =>
            depObj.SetValue(VisibilityReducedImageProperty, value);

        #endregion

        public ScrollImageViewer()
        {
            this.Loaded += (_, __) =>
            {
                var scrollContentPresenter = ViewHelper.GetChildControl<ScrollContentPresenter>(this);
                InsideScrollPresenter = scrollContentPresenter;

                // 初期サイズはLoadedで取得しようとしたけどイベント来ないのでココで
                //scrollContentPresenter.Loaded += (sender, e) => 
                ScrollContentActualSize.Value = ViewHelper.GetControlActualSize(scrollContentPresenter);
                scrollContentPresenter.SizeChanged += (sender, e) =>
                {
                    ScrollContentActualSize.Value = e.NewSize; //=ActualSize
                };


                var mainImage = ViewHelper.GetChildControl<Image>(this);
                if (mainImage is null) throw new NullReferenceException("Not Found Image Control. Invalid xaml.");
                InsideImage = mainImage;

                mainImage.TargetUpdated += (___, e) =>
                {
                    if (!(e.OriginalSource is Image image)) return;
                    ImageSourcePixelSize.Value = ViewHelper.GetImageSourcePixelSize(image);
                };
                mainImage.SizeChanged += (sender, e) =>
                {
                    ImageViewActualSize.Value = e.NewSize; //=ActualSize
                    MainImage_SizeChanged(sender, e);
                };


                if (VisualTreeHelper.GetParent(this) is Panel parentPanel)
                {
                    // サムネイルコントロール(Canvas)でもズーム操作を有効にするため、親パネルに添付イベントを貼る
                    parentPanel.AddHandler(PreviewMouseWheelEvent, new MouseWheelEventHandler(MainScrollViewer_PreviewMouseWheel));
                }
            };

            // ズーム倍率変更
            _ImageZoomMagni.CombineLatest(ImageSourcePixelSize, ScrollContentActualSize,
                    (zoomMag, imageSourceSize, scrollContentSize) => (zoomMag, imageSourceSize, scrollContentSize))
                .Subscribe(x => UpdateImageZoom(x.zoomMag, x.imageSourceSize, x.scrollContentSize));


            #region MouseWheel

            // マウスホイールによるズーム倍率変更
            MouseWheelZoomDelta
                .Where(x => x != 0)
                .Select(x => x > 0)
                .Subscribe(isZoomIn =>
                {
                    var oldImageZoomMag = ImageZoomMagni;

                    // ズーム前の倍率
                    double oldZoomMagRatio = GetCurrentZoomMagRatio(ImageViewActualSize.Value, ImageSourcePixelSize.Value);

                    // ズーム後のズーム管理クラス
                    var newImageZoomMag = oldImageZoomMag.ZoomMagnification(oldZoomMagRatio, isZoomIn);

                    // 全画面表示時を跨ぐ場合は全画面表示にする
                    var enrireZoomMag = GetEntireZoomMagRatio(ScrollContentActualSize.Value, ImageSourcePixelSize.Value);
                    if ((oldImageZoomMag.MagnificationRatio < enrireZoomMag && enrireZoomMag < newImageZoomMag.MagnificationRatio)
                        || (newImageZoomMag.MagnificationRatio < enrireZoomMag && enrireZoomMag < oldImageZoomMag.MagnificationRatio))
                    {
                        ImageZoomMagni = new ImageZoomMagnification(true, enrireZoomMag);
                    }
                    else
                    {
                        ImageZoomMagni = newImageZoomMag;
                    }
                });

            #endregion

        }

        #region ImageZoomMag

        private void UpdateImageZoom(ImageZoomMagnification zoomMagnification, Size imageSourceSize, Size scrollPresenterSize)
        {
            if (imageSourceSize.Width == 0.0 || imageSourceSize.Height == 0.0) return;

            if (!zoomMagnification.IsEntire)
            {
                // ズーム表示に切り替え
                // 画像サイズの更新前にスクロールバーの表示を更新(ContentSizeに影響出るので)
                UpdateScrollBarVisibility(this, zoomMagnification, scrollPresenterSize, imageSourceSize);

                InsideImage.Width = imageSourceSize.Width * zoomMagnification.MagnificationRatio;
                InsideImage.Height = imageSourceSize.Height * zoomMagnification.MagnificationRatio;
            }
            else
            {
                // 全画面表示に切り替え
                // 画像サイズの更新前にスクロールバーの表示を更新(ContentSizeに影響出るので)
                UpdateScrollBarVisibility(this, zoomMagnification, scrollPresenterSize, imageSourceSize);

                var size = GetEntireZoomSize(scrollPresenterSize, imageSourceSize);
                InsideImage.Width = size.Width;
                InsideImage.Height = size.Height;
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

        private void MainScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                MouseWheelZoomDelta.Value = e.Delta;

                // 最大ズームでホイールすると画像の表示エリアが移動しちゃうので止める
                e.Handled = true;
            }
        }

        #endregion

        #region ImageSizeChanged

        // 画像のサイズ変更(ズーム操作)
        private void MainImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var imageViewActualSize = e.NewSize;

            // 全画面表示よりもズームしてるかフラグ(e.NewSize == Size of MainImage)
            // 小数点以下がちょいずれして意図通りの判定にならないことがあるので整数化する
            bool isZoomOverEntire = (Math.Floor(imageViewActualSize.Width) > Math.Floor(ScrollContentActualSize.Value.Width)
                || Math.Floor(imageViewActualSize.Height) > Math.Floor(ScrollContentActualSize.Value.Height));

            // 全画面よりズームインしてたら縮小画像を表示
            SetVisibilityReducedImage(this, isZoomOverEntire ? Visibility.Visible : Visibility.Collapsed);

            //// 全画面よりズームアウトしたらスクロールバー位置を初期化
            //if (!isZoomOverEntire) ImageScrollOffsetRatio.Value = DefaultScrollOffsetRatio; ♪

            //// View→ViewModel
            //var magRatio = GetCurrentZoomMagRatio(imageViewActualSize, ImageSourcePixelSize.Value); ♪
            //var payload = new ImageZoomPayload(MainScrollViewer.ImageZoomMagni.IsEntire, magRatio); ♪
            //SetZoomPayload(MainScrollViewer, payload); ♪
        }

        #endregion
    }
}
