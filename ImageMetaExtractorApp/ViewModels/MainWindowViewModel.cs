using ImageMetaExtractorApp.Models;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ImageMetaExtractorApp.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private const string ImageSource = @"C:\data\ext\Image1.JPG";
        private static ImageMetas ImageMetas { get; } = new ImageMetas(ImageSource);

        /// <summary>
        /// 1画像のメタ情報
        /// </summary>
        public ReadOnlyObservableCollection<MetaItem> ImageMetasSource { get; }

        public MainWindowViewModel()
        {
            var metaItemList = ImageMetas.MetaItemLists[0];

            //var a = metaItemList.Select(x => new MetaItem(x));
            //ImageMetasSource = metaItemList.ToReadOnlyReactiveCollection<MetaItem>(x => new MetaItemVM(x));

            ImageMetasSource = metaItemList.ToReadOnlyReactiveCollection(x => x);
        }

    }

}
