using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using ZoomThumb.Models;

namespace ZoomThumb.ViewModels
{
    class ImageScrollViewerViewModel : BindableBase
    {
        private static readonly string ImagePath = @"C:\data\Image1.JPG";

        // 主画像
        public ReactiveProperty<BitmapSource> ImageSource { get; } =
            new ReactiveProperty<BitmapSource>(ImagePath.ToBitmapImage());

        // 主画像のサイズ
        public ReactiveProperty<Size> ImageViewSize { get; } = new ReactiveProperty<Size>();

        // ScrollViewerコントロールのサイズ
        public ReactiveProperty<Size> ScrollViewerSize { get; } = new ReactiveProperty<Size>(mode: ReactivePropertyMode.None);

        // ScrollViewerコントロールのサイズ(スクロールバー除く)
        public ReactiveProperty<Size> ScrollViewerContentSize { get; } = new ReactiveProperty<Size>(mode: ReactivePropertyMode.None);

        // ◆使ってない
        public ReactiveProperty<Size> HorizontalScrollBarSize { get; } = new ReactiveProperty<Size>();
        public ReactiveProperty<bool> HorizontalScrollBarVisible { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<Size> VerticalScrollBarSize { get; } = new ReactiveProperty<Size>();
        public ReactiveProperty<bool> VerticalScrollBarVisible { get; } = new ReactiveProperty<bool>();

        // スクロールバーの強制非表示フラグ(ズーム表示からの全画面表示でスクロールバーが消えない対策)
        private bool _ScrollBarForceVisibilityCollapsed;
        public bool ScrollBarForceVisibilityCollapsed
        {
            get => _ScrollBarForceVisibilityCollapsed;
            private set => SetProperty(ref _ScrollBarForceVisibilityCollapsed, value);
        }

        public ReactiveProperty<Size> ScrollOffset { get; } = new ReactiveProperty<Size>();

        public ReactiveProperty<Unit> ScrollViewerContentMouseLeftDownImage { get; } = new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);
        public ReactiveProperty<Unit> ScrollViewerContentMouseLeftUpImage { get; } = new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);
        public ReactiveProperty<Point> ScrollViewerContentMouseMove { get; } = new ReactiveProperty<Point>();

        public ReactiveProperty<Unit> ScrollViewerMouseDoubleClick { get; } = new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);

        public ReactiveProperty<Unit> ScrollViewerMouseLeftDownImage { get; } = new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);
        public ReactiveProperty<Unit> ScrollViewerMouseLeftUpImage { get; } = new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);
        public ReactiveProperty<Point> ScrollViewerMouseMove { get; } = new ReactiveProperty<Point>();

        public ReactiveProperty<int> ScrollViewerMouseWheel { get; } = new ReactiveProperty<int>(mode: ReactivePropertyMode.None);

        // ズーム倍率の管理
        private readonly ReactiveProperty<ImageZoomMagnification> ImageZoomMag =
            new ReactiveProperty<ImageZoomMagnification>(ImageZoomMagnification.Entire);

        public ReadOnlyReactiveProperty<bool> BitmapScalingModeResolution { get; }

        public ImageScrollViewerViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            // ズーム表示
            ImageZoomMag
                .Where(x => !x.IsEntire)
                .Select(x => x.MagnificationRatio)
                .Subscribe(magRatio =>
                {
                    // ズーム表示からの全画面表示でスクロールバーが消えない対策
                    //   全画面表示中はスクロールバーを非表示にしているので、ズーム表示中は表示に戻す
                    ScrollBarForceVisibilityCollapsed = false;
                    ImageViewSize.Value = new Size(
                        ImageSource.Value.PixelWidth * magRatio,
                        ImageSource.Value.PixelHeight * magRatio);
                });

            // 全画面表示
            ImageZoomMag
                .CombineLatest(ScrollViewerSize, (mag, size) => (mag, size))
                .Where(x => x.mag.IsEntire)
                .Select(x => x.size)
                .Subscribe(x =>
                {
                    // ズーム表示からの全画面表示でスクロールバーが消えない対策
                    //   スクロールバーが表示された状態で、親コントロール(ScrollViewer)とジャストのサイズに設定すると、
                    //   自動でスクロールバーが消えないので明示的に消す
                    ScrollBarForceVisibilityCollapsed = true;
                    ImageViewSize.Value = GetEntireImageViewSize(ImageSource.Value, x);
                    //Console.WriteLine($"isZoomRatioAll: ({ImageViewSize.Value.Width}, {ImageViewSize.Value.Height})");
                });

            // Debug
            //HorizontalScrollBarSize.Select(x => x.Height).Subscribe(x => Console.WriteLine($"Hori: {x}"));
            //HorizontalScrollBarVisible.Subscribe(x => Console.WriteLine($"HorB: {x}"));
            //VerticalScrollBarSize.Select(x => x.Width).Subscribe(x => Console.WriteLine($"Vert: {x}"));
            //VerticalScrollBarVisible.Subscribe(x => Console.WriteLine($"VerB: {x}"));

            #region DoubleClickZoom

            // 表示状態を切り替える
            ScrollViewerMouseDoubleClick.Subscribe(_ => SwitchClickZoomMag());

            #endregion

            #region SingleClickZoom

            // 一時ズームフラグ
            var temporaryZoomSubject = new ReactivePropertySlim<bool>(false, mode: ReactivePropertyMode.DistinctUntilChanged);

            // 長押しによる一時ズーム
            ScrollViewerMouseLeftDownImage
                .Throttle(TimeSpan.FromMilliseconds(300))   // 長押し判定
                .TakeUntil(ScrollViewerMouseLeftUpImage)    // 押下中のみ対象(ちょん離し後なら弾く)
                .Repeat()
                .Where(_ => ImageZoomMag.Value.IsEntire)    // 全体表示なら流す(継続ズームを弾くため既にズームしてたら流さない)
                .Subscribe(_ => temporaryZoomSubject.Value = true);

            // 一時ズーム解除
            ScrollViewerMouseLeftUpImage
                .Where(_ => temporaryZoomSubject.Value)     // 一時ズームなら解除する(継続ズームは解除しない)
                .Subscribe(_ => temporaryZoomSubject.Value = false);

            temporaryZoomSubject.Subscribe(_ => SwitchClickZoomMag());

            #endregion

            #region MouseMove

            // マウス押下中のみマウスの移動量を流す(SingleClickもここを通っっちゃてる。想定外やけどまぁOK)
            ScrollViewerContentMouseMove
                .Pairwise()                                         // 最新値と前回値を取得
                .Select(x => -(x.NewItem - x.OldItem))              // 引っ張りと逆方向なので反転
                .SkipUntil(ScrollViewerContentMouseLeftDownImage)
                .TakeUntil(ScrollViewerContentMouseLeftUpImage)
                .Repeat()
                //.Where(_ => !ImageZoomMag.Value.IsEntire)           // ズーム中のみ流す(全画面表示中は画像移動不要)
                .Where(_ => !temporaryZoomSubject.Value)            // 一時ズームは移動させない仕様
                .Subscribe(v => ScrollOffset.Value = ShiftDraggingScrollOffset(ScrollOffset.Value, ImageViewSize.Value, ScrollViewerContentSize.Value, v));

            #endregion

            #region MouseWheelZoom

            // マウスホイールによるズーム倍率変更
            ScrollViewerMouseWheel
                .Where(x => x != 0)
                .Subscribe(x =>
                {
                    double oldZoomMag = GetCurrentZoomMagnificationRatio();  // ズーム前の倍率

                    // ズーム後のズーム管理クラス
                    ImageZoomMagnification imageZoomMag;
                    if (x > 0) imageZoomMag = ImageZoomMag.Value.ZoomInMagnification(oldZoomMag);
                    else imageZoomMag = ImageZoomMag.Value.ZoomOutMagnification(oldZoomMag);

                    // 現在のスクロールバーの中央位置
                    var scrollOffsetCenterRatio = GetScrollOffsetCenterRatio();

                    // ズーム後の画像サイズ
                    double newZoomMag = imageZoomMag.MagnificationRatio;
                    var newImageSize = new Size(
                        ImageViewSize.Value.Width * newZoomMag / oldZoomMag,
                        ImageViewSize.Value.Height * newZoomMag / oldZoomMag);

                    // scrollOffsetCenterRatioが0~1の範囲内なので最大側は制限不要(値に変化ある場合のみ設定)
                    ScrollOffset.Value = new Size(
                        Math.Max(0.0, scrollOffsetCenterRatio.Width * newImageSize.Width - (ScrollViewerSize.Value.Width / 2.0)),
                        Math.Max(0.0, scrollOffsetCenterRatio.Height * newImageSize.Height - (ScrollViewerSize.Value.Height / 2.0)));

                    // ズーム倍率の更新(オフセット更新後に実施)
                    ImageZoomMag.Value = imageZoomMag;

                    Console.WriteLine($"Mag: {oldZoomMag:f2} => {ImageZoomMag.Value.MagnificationRatio:f2}");
                });

            // レンダリングオプションの指定(100%以上の拡大ズームならPixelが見える設定にする)
            BitmapScalingModeResolution = ImageViewSize
                .CombineLatest(ImageSource, (x, y) => new { View = x, Source = new Size(y.PixelWidth, y.PixelHeight) })
                .Select(x => (x.View.Width < x.Source.Width) || (x.View.Height < x.Source.Height))
                .ToReadOnlyReactiveProperty();

            #endregion

        }

        // 現在の画像のズーム倍率を返す
        private double GetCurrentZoomMagnificationRatio()
        {
            // 全画面表示でなければ倍率が入っているのでそのまま返す
            if (!ImageZoomMag.Value.IsEntire) return ImageZoomMag.Value.MagnificationRatio;

            return Math.Min(
                ImageViewSize.Value.Width / ImageSource.Value.PixelWidth,
                ImageViewSize.Value.Height / ImageSource.Value.PixelHeight);
        }

        // 現在のスクロールバーの中央位置
        private Size GetScrollOffsetCenterRatio()
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            var scviewSize = ScrollViewerSize.Value;
            var scviewBarPoint = ScrollOffset.Value;
            var imageSize = ImageViewSize.Value;

            double w = scviewBarPoint.Width + (scviewSize.Width / 2.0);
            if (scviewSize.Width > imageSize.Width)
                w -= (scviewSize.Width - imageSize.Width) / 2.0;
            w /= imageSize.Width;

            double h = scviewBarPoint.Height + (scviewSize.Height / 2.0);
            if (scviewSize.Height > imageSize.Height)
                h -= (scviewSize.Height - imageSize.Height) / 2.0;
            h /= imageSize.Height;

            var centRate = new Size(clip(w, 0.0, 1.0), clip(h, 0.0, 1.0));
            //Console.WriteLine($"**scviewSize: {scviewSize}  scviewBarPoint: {scviewBarPoint}  imageSize: {imageSize}");
            //Console.WriteLine($"**CenterRate: {centRate}");

            return centRate;
        }

        // 画像全体表示のViewサイズ
        private static Size GetEntireImageViewSize(BitmapSource imageSource, Size scrollViewerSize)
        {
            if (imageSource.PixelHeight == 0) throw new DivideByZeroException(nameof(imageSource.PixelHeight));
            if (scrollViewerSize.Height == 0) throw new DivideByZeroException(nameof(scrollViewerSize.Height));

            var imageRatio = (double)imageSource.PixelWidth / imageSource.PixelHeight;
            if (imageRatio > scrollViewerSize.Width / scrollViewerSize.Height)
            {
                // 横パンパン
                return new Size(scrollViewerSize.Width, scrollViewerSize.Width / imageRatio);
            }
            else
            {
                // 縦パンパン
                return new Size(scrollViewerSize.Height * imageRatio, scrollViewerSize.Height);
            }
        }

        // マウスドラッグ中の表示位置のシフト
        private static Size ShiftDraggingScrollOffset(Size offsetSize, Size imageSize, Size sviewSize, Vector shift)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            double widthMax = Math.Max(0.0, imageSize.Width - sviewSize.Width);
            double heightMax = Math.Max(0.0, imageSize.Height - sviewSize.Height);

            return new Size(
                clip(offsetSize.Width + shift.X, 0.0, widthMax),
                clip(offsetSize.Height + shift.Y, 0.0, heightMax));
        }

        // クリックズームの状態を切り替える(全画面⇔等倍)
        private void SwitchClickZoomMag()
        {
            var mag = ImageZoomMag.Value.MagnificationToggle();

            // ズーム表示への切り替えならスクロールバーを移動(ImageViewSizeを変更する前に実施する)
            if (!mag.IsEntire) ScrollOffset.Value = GetClickZoomScrollOffset(mag.MagnificationRatio);

            ImageZoomMag.Value = mag;
        }

        // クリックズーム時のスクロールオフセットを取得
        private Size GetClickZoomScrollOffset(double magRatio)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            // 親ScrollViewerから子Imageまでのサイズ
            var imageControlSizeOffset = new Size(
                Math.Max(0, ScrollViewerSize.Value.Width - ImageViewSize.Value.Width) / 2.0,
                Math.Max(0, ScrollViewerSize.Value.Height - ImageViewSize.Value.Height) / 2.0);

            // 子Image基準のマウス位置
            var mousePos = new Point(
                Math.Max(0, ScrollViewerContentMouseMove.Value.X - imageControlSizeOffset.Width),
                Math.Max(0, ScrollViewerContentMouseMove.Value.Y - imageControlSizeOffset.Height));

            // ズーム後の中心座標の割合
            var zoomCenterRate = new Point(
                clip(mousePos.X / ImageViewSize.Value.Width, 0.0, 1.0),
                clip(mousePos.Y / ImageViewSize.Value.Height, 0.0, 1.0));

            // ズーム後の全画像サイズ
            var zoomImageSize = new Size(
                ImageSource.Value.PixelWidth * magRatio,
                ImageSource.Value.PixelHeight * magRatio);

            // ズーム後の表示領域の半分の割合
            var displayRateHalf = new Size(
                ScrollViewerSize.Value.Width / zoomImageSize.Width / 2.0,
                ScrollViewerSize.Value.Height / zoomImageSize.Height / 2.0);

            // ズーム後の左上座標の割合
            var zoomRateLeftTop = new Point(
                clip(zoomCenterRate.X - displayRateHalf.Width, 0.0, 1.0),
                clip(zoomCenterRate.Y - displayRateHalf.Height, 0.0, 1.0));

            var zoomNewSize = new Size(
                zoomRateLeftTop.X * zoomImageSize.Width,
                zoomRateLeftTop.Y * zoomImageSize.Height);

            return zoomNewSize;
        }

        // 画像の倍率管理
        struct ImageZoomMagnification
        {
            private static readonly double MagRatioMin = Math.Pow(2, -5);   // 3.1%
            private static readonly double MagRatioMax = Math.Pow(2, 5);    // 3200%
            private static readonly double MagStep = 2.0;                   // 2倍

            public bool IsEntire { get; private set; }
            public double MagnificationRatio { get; private set; }

            private ImageZoomMagnification(bool flag)
            {
                if (!flag) throw new ArgumentException(nameof(flag));
                IsEntire = true;
                MagnificationRatio = double.NaN;
            }

            private ImageZoomMagnification(double mag)
            {
                IsEntire = false;
                MagnificationRatio = mag;
            }

            public static ImageZoomMagnification Entire = new ImageZoomMagnification(true);
            private static ImageZoomMagnification MagX1 = new ImageZoomMagnification(1.0);

            public ImageZoomMagnification MagnificationToggle() => IsEntire ? MagX1 : Entire;

            private ImageZoomMagnification ZoomMagnification(double currentMag, double ratio)
            {
                // ホイールすると2の冪乗になるよう元の倍率を補正する
                double currentMagPowerRaw = Math.Log(currentMag) / Math.Log(2);
                double currentMagRound = Math.Pow(2, Math.Round(currentMagPowerRaw));

                double newMag = currentMagRound * ratio;
                if (newMag < MagRatioMin) newMag = MagRatioMin;
                else if (newMag > MagRatioMax) newMag = MagRatioMax;
                return new ImageZoomMagnification(newMag);
            }

            public ImageZoomMagnification ZoomInMagnification(double currentMag) =>
                ZoomMagnification(currentMag, MagStep);

            public ImageZoomMagnification ZoomOutMagnification(double currentMag) =>
                ZoomMagnification(currentMag, 1.0 / MagStep);

        }

    }
}
