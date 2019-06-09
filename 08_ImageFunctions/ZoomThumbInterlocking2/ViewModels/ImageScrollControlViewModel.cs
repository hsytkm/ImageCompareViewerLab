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
    class ImageScrollControlViewModel : BindableBase
    {
        // 主画像
        public ReadOnlyReactiveProperty<BitmapSource> ImageSource { get; }

        // 各画像の表示エリアを連動させるかフラグ(FALSE=連動しない)
        public ReadOnlyReactiveProperty<bool> IsImageViewerInterlock { get; }

        // 縮小画像の表示可能フラグ(FALSE=表示禁止)
        public ReadOnlyReactiveProperty<bool> CanVisibleReducedImage { get; }

        // 画像上のサンプリング枠の表示フラグ(FALSE=表示しない)
        public ReadOnlyReactiveProperty<bool> IsVisibleImageSamplingFrame { get; }
        
        // ズーム倍率の管理(TwoWay)
        public ReactiveProperty<ImageZoomPayload> ImageZoomPayload { get; } =
            new ReactiveProperty<ImageZoomPayload>(mode: ReactivePropertyMode.DistinctUntilChanged);
        
        // スクロールオフセット位置(TwoWay)
        public ReactiveProperty<Point> ImageScrollOffsetCenterRatio { get; } =
            new ReactiveProperty<Point>(mode: ReactivePropertyMode.DistinctUntilChanged);

        // サンプリング枠の位置(TwoWay)
        public ReactiveProperty<Rect> SamplingFrameRectRatio { get; } =
            new ReactiveProperty<Rect>(mode: ReactivePropertyMode.DistinctUntilChanged);

        public ReactiveCommand LoadImageCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ZoomAllCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ZoomX1Command { get; } = new ReactiveCommand();
        public ReactiveCommand OffsetCenterCommand { get; } = new ReactiveCommand();

        public ReactiveProperty<Size> SizeTest { get; } = new ReactiveProperty<Size>();
        public ReactiveProperty<Point> PointTest { get; } = new ReactiveProperty<Point>();

        public ImageScrollControlViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            var mainImages = container.Resolve<MainImages>();
            var viewSettings = container.Resolve<ViewSettings>();

            IsImageViewerInterlock = viewSettings.ObserveProperty(x => x.IsImageViewerInterlock).ToReadOnlyReactiveProperty();
            CanVisibleReducedImage = viewSettings.ObserveProperty(x => x.CanVisibleReducedImage).ToReadOnlyReactiveProperty();
            IsVisibleImageSamplingFrame = viewSettings.ObserveProperty(x => x.IsVisibleImageOverlapSamplingFrame).ToReadOnlyReactiveProperty();

            // 画像管理クラスのインデックスを取得
            int index = mainImages.GetImageIndex();

            LoadImageCommand.Subscribe(x => mainImages.LoadImage(index));

            ImageSource = mainImages.ImageSources[index].ObserveProperty(x => x.ImageSource)
                .ToReadOnlyReactiveProperty(mode: ReactivePropertyMode.None);

            // View通知情報のデバッグ表示
            ImageZoomPayload
                .Subscribe(x => Console.WriteLine($"VM({index})-ZoomMagRatio: {x.IsEntire} => {(x.MagRatio * 100.0):f2} %"));

            ImageScrollOffsetCenterRatio
                .Subscribe(x => Console.WriteLine($"VM({index})-ScrollOffsetRatio: ({x.X:f2}, {x.Y:f2})"));

            SamplingFrameRectRatio
                .Subscribe(x => Console.WriteLine($"VM({index})-FrameRectRatio: ({x.X:f2}, {x.Y:f2}) {x.Width:f2} x {x.Height:f2}"));

            ZoomAllCommand
                .Subscribe(x =>
                {
                    // 全画面の再要求を行うと、Viewで設定した倍率をクリアしてしまうので行わない
                    if (!ImageZoomPayload.Value.IsEntire)
                        ImageZoomPayload.Value = new ImageZoomPayload(true, double.NaN);
                });

            ZoomX1Command
                .Subscribe(x => ImageZoomPayload.Value = new ImageZoomPayload(false, 1.0));

            OffsetCenterCommand
                .Subscribe(x => ImageScrollOffsetCenterRatio.Value = new Point(0.5, 0.5));


            //SizeTest.Subscribe(x => Console.WriteLine($"VM-SizeTest: {x}"));
            //PointTest.Subscribe(x => Console.WriteLine($"VM-PointTest: {x}"));

        }

    }

}
