using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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

        // 表示切替ボタン
        //public ReactiveCommand ZoomX1Command { get; }
        //public ReactiveCommand ZoomAllCommand { get; }

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

        public ImageScrollViewerViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            // 等倍表示中フラグ
            var isZoomRatioAll = new ReactivePropertySlim<bool>(true);

            // 等倍表示コマンド
            //ZoomX1Command = new ReactiveCommand(isZoomRatioAll);
            //ZoomX1Command.Subscribe(_ => isZoomRatioAll.Value = false);

            // 等倍表示
            isZoomRatioAll.Where(x => !x)
                .Subscribe(x =>
                {
                    double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

                    double zoomRate = 1.00;

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
                        ImageSource.Value.PixelWidth * zoomRate,
                        ImageSource.Value.PixelHeight * zoomRate);

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

                    // 自コントロールへの要求
                    ScrollOffset.Value = zoomNewSize;



                    // ズーム表示からの全画面表示でスクロールバーが消えない対策
                    //   全画面表示中はスクロールバーを非表示にしているので、ズーム表示中は表示に戻す
                    ScrollBarForceVisibilityCollapsed = false;
                    ImageViewSize.Value = new Size(ImageSource.Value.PixelWidth, ImageSource.Value.PixelHeight);

                });

            // 全画面表示コマンド
            //ZoomAllCommand = new ReactiveCommand(isZoomRatioAll.Select(x => !x));
            //ZoomAllCommand.Subscribe(_ => isZoomRatioAll.Value = true);

            // 全画面表示
            isZoomRatioAll
                .CombineLatest(ScrollViewerSize, (b, s) => (b, s))
                .Where(x => x.b).Select(x => x.s)
                .Subscribe(x =>
                {
                    // ズーム表示からの全画面表示でスクロールバーが消えない対策
                    //   スクロールバーが表示された状態で、親コントロール(ScrollViewer)とジャストのサイズに設定すると、
                    //   自動でスクロールバーが消えないので明示的に消す
                    ScrollBarForceVisibilityCollapsed = true;
                    ImageViewSize.Value = GetImageViewSize(ImageSource.Value, x);
                    Console.WriteLine($"isZoomRatioAll: ({ImageViewSize.Value.Width}, {ImageViewSize.Value.Height})");

                    Size GetImageViewSize(BitmapSource image, Size scrollViewerSize)
                    {
                        var imageRatio = (double)image.PixelWidth / image.PixelHeight;
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
                });

            // Debug
            //HorizontalScrollBarSize.Select(x => x.Height).Subscribe(x => Console.WriteLine($"Hori: {x}"));
            //HorizontalScrollBarVisible.Subscribe(x => Console.WriteLine($"HorB: {x}"));
            //VerticalScrollBarSize.Select(x => x.Width).Subscribe(x => Console.WriteLine($"Vert: {x}"));
            //VerticalScrollBarVisible.Subscribe(x => Console.WriteLine($"VerB: {x}"));

            #region DoubleClickZoom

            // 表示状態を切り替える
            ScrollViewerMouseDoubleClick.Subscribe(_ => isZoomRatioAll.Value = !isZoomRatioAll.Value);

            #endregion

            #region SingleClickZoom

            // 一時ズームフラグ
            var temporaryZoomSubject = new ReactivePropertySlim<bool>(false);

            // 長押しによる一時ズーム
            ScrollViewerMouseLeftDownImage
                .Throttle(TimeSpan.FromMilliseconds(300))   // 長押し判定
                .TakeUntil(ScrollViewerMouseLeftUpImage)    // 押下中のみ対象(ちょん離し後なら弾く)
                .Repeat()
                .Where(_ => isZoomRatioAll.Value)           // 既にズームしてたら入れない(継続ズームを弾く)
                .Subscribe(_ => temporaryZoomSubject.Value = true);

            // 一時ズーム解除
            ScrollViewerMouseLeftUpImage
                .Where(_ => temporaryZoomSubject.Value)     // 一時ズームなら解除する(継続ズームは解除しない)
                .Subscribe(_ => temporaryZoomSubject.Value = false);

            temporaryZoomSubject.Subscribe(x => isZoomRatioAll.Value = !x);
            
            #endregion

            #region MouseMove

            // マウス押下中のみマウスの移動量を流す(SingleClickもここを通っっちゃてる。想定外やけどまぁOK)
            ScrollViewerContentMouseMove
                .Pairwise()                                         // 最新値と前回値を取得
                .Select(x => -(x.NewItem - x.OldItem))              // 引っ張りと逆方向なので反転
                .SkipUntil(ScrollViewerContentMouseLeftDownImage)
                .TakeUntil(ScrollViewerContentMouseLeftUpImage)
                .Repeat()
                .Where(_ => !isZoomRatioAll.Value)                  // 全画面表示中は画像移動不要
                .Where(_ => !temporaryZoomSubject.Value)            // 一時ズームは移動させない仕様
                .Subscribe(v => ScrollOffset.Value = ShiftDraggingScrollOffset(ScrollOffset.Value, ImageViewSize.Value, ScrollViewerContentSize.Value, v));

            #endregion
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

    }
}
