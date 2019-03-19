using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ZoomThumb.Models;

namespace ZoomThumb.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public ReactiveCommand LoadImageCommand { get; } = new ReactiveCommand();

        public ReactiveProperty<IEnumerable<Uri>> DropEvent { get; } = new ReactiveProperty<IEnumerable<Uri>>();

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager, MyImage myImage)
        {
            LoadImageCommand.Subscribe(x => myImage.LoadImage());

            DropEvent.Select(x => x?.FirstOrDefault()?.LocalPath).Where(x => x != null)
                .Subscribe(x => myImage.LoadImage(x));
        }

    }
}
