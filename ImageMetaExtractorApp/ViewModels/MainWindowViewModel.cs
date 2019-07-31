using ImageMetaExtractorApp.Models;
using ImageMetaExtractorApp.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ImageMetaExtractorApp.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private const string ImageSource1 = @"C:\data\Image1.JPG";
        private const string ImageSource2 = @"C:\data\Image2.JPG";

        private readonly ImageMetas ImageMetas1;
        private readonly ImageMetas ImageMetas2;

        private readonly IRegionManager _regionManager;

        public DelegateCommand AddTab1Command { get; }
        public DelegateCommand AddTab2Command { get; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            ImageMetas1 = new ImageMetas(ImageSource1);
            ImageMetas2 = new ImageMetas(ImageSource2);

            AddTab1Command = new DelegateCommand(AddTab1);
            AddTab2Command = new DelegateCommand(AddTab2);
        }

        public void AddTab1() => AddTab(ImageMetas1);
        public void AddTab2() => AddTab(ImageMetas2);

        private void AddTab(ImageMetas imageMetas)
        {
            foreach (var metaItemGroup in imageMetas.MetaItemGroups)
            {
                if (metaItemGroup != null)
                {
                    var parameters = new NavigationParameters
                    {
                        { MetaTabDetailViewModel.MetaItemGroupKey, metaItemGroup }
                    };
                    _regionManager.RequestNavigate("MetaTabDetailsRegion", nameof(MetaTabDetail), parameters);
                }
            }
        }

    }

}
