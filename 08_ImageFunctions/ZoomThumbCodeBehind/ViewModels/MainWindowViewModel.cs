using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System;
using System.Reactive.Linq;
using ZoomThumb.Models;

namespace ZoomThumb.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public ReactiveCommand LoadImageCommand { get; } = new ReactiveCommand();

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager, MyImage myImage)
        {
            LoadImageCommand.Subscribe(x => myImage.LoadImage());
        }

    }
}
