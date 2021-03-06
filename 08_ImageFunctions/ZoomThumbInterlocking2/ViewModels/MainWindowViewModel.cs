﻿using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System;
using ZoomThumb.Models;

namespace ZoomThumb.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public ReactiveProperty<bool> IsImageViewerInterlock { get; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> CanVisibleReducedImage { get; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> IsVisibleImageOverlapSamplingFrame { get; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> IsSamplingFrameOnImage { get; } = new ReactiveProperty<bool>(true);

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            var viewSettings = container.Resolve<ViewSettings>();

            IsImageViewerInterlock.Subscribe(x => viewSettings.IsImageViewerInterlock = x);
            CanVisibleReducedImage.Subscribe(x => viewSettings.CanVisibleReducedImage = x);
            IsVisibleImageOverlapSamplingFrame.Subscribe(x => viewSettings.IsVisibleImageOverlapSamplingFrame = x);
            IsSamplingFrameOnImage.Subscribe(x => viewSettings.IsSamplingFrameOnImage = x);

        }

    }
}
