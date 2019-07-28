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
        private const string ImageSource1 = @"C:\data\ext\Image1.JPG";

        private readonly IRegionManager _regionManager;

        public DelegateCommand AddTabCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            AddTabCommand = new DelegateCommand(AddTab);
        }

        public void AddTab()
        {
            var imageMetas = new ImageMetas(ImageSource1);
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
