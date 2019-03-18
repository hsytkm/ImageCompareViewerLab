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
        // 主画像
        public ReadOnlyReactiveProperty<BitmapSource> ImageSource { get; }

        // 主画像のサイズ
        public ReactiveProperty<double> ImageViewWidth { get; } = new ReactiveProperty<double>();
        public ReactiveProperty<double> ImageViewHeight { get; } = new ReactiveProperty<double>();
        private Size ImageViewSize { get => new Size(ImageViewWidth.Value, ImageViewHeight.Value); }

        // ScrollViewerコントロールのサイズ
        public ReactiveProperty<Size> ScrollViewerSize { get; } = new ReactiveProperty<Size>(mode: ReactivePropertyMode.None);
        
        public ReactiveProperty<Unit> ScrollViewerContentMouseLeftDownImage { get; } = new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);
        public ReactiveProperty<Unit> ScrollViewerContentMouseLeftUpImage { get; } = new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);
        public ReactiveProperty<Point> ScrollViewerContentMouseMove { get; } = new ReactiveProperty<Point>();

        public ReactiveProperty<Unit> ScrollViewerMouseDoubleClick { get; } = new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);

        // ズーム倍率の管理(OneWayやけどTwoWayにしたい)
        public ReactiveProperty<ImageZoomMagnification> ImageZoomMag { get; } = new ReactiveProperty<ImageZoomMagnification>();

        // スクロールオフセット位置(TwoWay)
        public ReactiveProperty<Size> ImageScrollOffset { get; } = new ReactiveProperty<Size>(new Size(0.5, 0.5));

        public ImageScrollViewerViewModel(IContainerExtension container, IRegionManager regionManager, MyImage myImage)
        {
            ImageSource = myImage.ObserveProperty(x => x.ImageSource).ToReadOnlyReactiveProperty(mode: ReactivePropertyMode.None);

            // 画像読み込み直後は全画面表示にしといてみる
            ImageSource.Subscribe(x => ImageZoomMag.Value = ImageZoomMagnification.Entire);

            #region DoubleClickZoom

            // 表示状態を切り替える
            ScrollViewerMouseDoubleClick.Subscribe(_ => SwitchClickZoomMag());

            #endregion

            #region SingleClickZoom

            // 一時ズームフラグ
            var temporaryZoom = new ReactivePropertySlim<bool>(false, mode: ReactivePropertyMode.DistinctUntilChanged);

            // 長押しによる一時ズーム
            ScrollViewerContentMouseLeftDownImage               //ScrollViewerMouseLeftDownImage
                .Throttle(TimeSpan.FromMilliseconds(300))       // 長押し判定
                .TakeUntil(ScrollViewerContentMouseLeftUpImage) // 押下中のみ対象(ちょん離し後なら弾く)　ScrollViewerMouseLeftUpImage
                .Repeat()
                .Where(_ => ImageZoomMag.Value.IsEntire)        // 全体表示なら流す(継続ズームを弾くため既にズームしてたら流さない)
                .Subscribe(_ => temporaryZoom.Value = true);

            // 一時ズーム解除
            ScrollViewerContentMouseLeftUpImage                 //ScrollViewerMouseLeftUpImage
                .Where(_ => temporaryZoom.Value)                // 一時ズームなら解除する(継続ズームは解除しない)
                .Subscribe(_ => temporaryZoom.Value = false);

            temporaryZoom.Subscribe(_ => SwitchClickZoomMag());

            #endregion

            #region MouseMove

            // マウス押下中のみマウスの移動割合をViewに流す
            ScrollViewerContentMouseMove
                .Pairwise()                                         // 最新値と前回値を取得
                .Select(x => -(x.NewItem - x.OldItem))              // 引っ張りと逆方向なので反転
                .SkipUntil(ScrollViewerContentMouseLeftDownImage)
                .TakeUntil(ScrollViewerContentMouseLeftUpImage)
                .Repeat()
                .Where(_ => !ImageZoomMag.Value.IsEntire)           // ズーム中のみ流す(全画面表示中は画像移動不要)
                .Where(_ => !temporaryZoom.Value)                   // ◆一時ズームは移動させない仕様
                .Subscribe(v => ImageScrollOffset.Value = ShiftDraggingScrollOffset(ImageScrollOffset.Value, ImageViewSize, v));

            #endregion

        }

        // マウスドラッグ中の表示位置のシフト
        private Size ShiftDraggingScrollOffset(Size offsetSize, Size imageSize, Vector shift)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            var shiftRate = new Vector(shift.X / imageSize.Width, shift.Y / imageSize.Height);

            return new Size(
                clip(offsetSize.Width + shiftRate.X, 0.0, 1.0),
                clip(offsetSize.Height + shiftRate.Y, 0.0, 1.0));
        }

        // クリックズームの状態を切り替える(全画面⇔等倍)
        private void SwitchClickZoomMag()
        {
            var mag = ImageZoomMag.Value.MagnificationToggle();

            ImageZoomMag.Value = mag;

            // ズーム表示への切り替えならスクロールバーを移動(ImageViewSizeを変更した後に実施する)
            if (!mag.IsEntire)
            {
                var imageViewSize = ImageViewSize;
                var scrollViewerSize = ScrollViewerSize.Value;
                var imageSourceSize = new Size(ImageSource.Value.PixelWidth, ImageSource.Value.PixelHeight);
                var scrollVieweMousePoint = ScrollViewerContentMouseMove.Value;

                ImageScrollOffset.Value = GetClickZoomScrollOffset(
                    mag.MagnificationRatio, imageViewSize, scrollViewerSize, scrollVieweMousePoint);
            }
        }

        // クリックズーム時のスクロールオフセットを取得
        private static Size GetClickZoomScrollOffset(double magRatio, Size imageViewSize, Size scrollViewerSize, Point scrollVieweMousePoint)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

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
            
            return zoomCenterRate;
        }
    }

    // 画像の倍率管理
    public struct ImageZoomMagnification
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

        public ImageZoomMagnification ZoomMagnification(double currentMag, bool isZoomIn)
        {
            var step = isZoomIn ? MagStep : 1.0 / MagStep;
            return ZoomMagnification(currentMag, step);
        }

    }

}
