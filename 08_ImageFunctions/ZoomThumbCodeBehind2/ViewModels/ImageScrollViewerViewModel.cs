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
        public ReactiveProperty<ImageZoomPayload> ImageZoomPayload { get; } = new ReactiveProperty<ImageZoomPayload>(mode: ReactivePropertyMode.DistinctUntilChanged);
        

        // スクロールオフセット位置(OneWayToSource)
        public ReactiveProperty<Size> ImageScrollOffset { get; } = new ReactiveProperty<Size>(new Size(0.5, 0.5));

        public ImageScrollViewerViewModel(IContainerExtension container, IRegionManager regionManager, MyImage myImage)
        {
            ImageSource = myImage.ObserveProperty(x => x.ImageSource).ToReadOnlyReactiveProperty(mode: ReactivePropertyMode.None);

            // ズーム倍率のデバッグ表示
            ImageZoomPayload
                .Subscribe(x => Console.WriteLine($"VM-ZoomMag: {x.IsEntire} => {x.MagRatio:f2}"));

            // スクロール位置のデバッグ表示
            ImageScrollOffset.Subscribe(x => Console.WriteLine($"VM-ScrollOffset: {x.Width:f2} x {x.Height:f2}"));
            
        }
        
    }

}
