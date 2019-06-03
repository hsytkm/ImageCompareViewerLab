using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System;
using ZoomThumb.Models;

namespace ZoomThumb.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public ReactiveProperty<bool> IsImageViewerInterlock { get; set; } = new ReactiveProperty<bool>(true);

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            var mainImages = container.Resolve<MainImages>();

            IsImageViewerInterlock.Subscribe(x => mainImages.IsImageViewerInterlock = x);
        }

    }
}
