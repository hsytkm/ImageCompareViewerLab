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

        // ズーム倍率の管理(今はOneWayToSource)
        public ReactiveProperty<ImageZoomPayload> ImageZoomPayload { get; } =
            new ReactiveProperty<ImageZoomPayload>(mode: ReactivePropertyMode.DistinctUntilChanged);
        
        // スクロールオフセット位置(OneWayToSource)
        public ReactiveProperty<Size> ImageScrollOffsetCenter { get; } =
            new ReactiveProperty<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);

        public ReactiveCommand LoadImageCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ZoomAllCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ZoomX1Command { get; } = new ReactiveCommand();
        public ReactiveCommand OffsetCenterCommand { get; } = new ReactiveCommand();

        public ImageScrollViewerViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            var mainImages = container.Resolve<MainImages>();

            // 画像管理クラスのインデックスを取得
            int index = mainImages.GetImageIndex();

            LoadImageCommand.Subscribe(x => mainImages.LoadImage(index));

            ImageSource = mainImages.ImageSources[index].ObserveProperty(x => x.ImageSource)
                .ToReadOnlyReactiveProperty(mode: ReactivePropertyMode.None);

            // ズーム倍率のデバッグ表示
            ImageZoomPayload
                .Subscribe(x => Console.WriteLine($"VM({index})-ZoomMagRatio: {x.IsEntire} => {(x.MagRatio * 100.0):f2} %"));

            // スクロール位置のデバッグ表示
            ImageScrollOffsetCenter
                .Subscribe(x => Console.WriteLine($"VM({index})-ScrollOffsetRatio: {x.Width:f2} x {x.Height:f2}"));


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
                .Subscribe(x => ImageScrollOffsetCenter.Value = new Size(0.5, 0.5));

        }

    }

}
