using Prism.Mvvm;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media.Imaging;
using VirtualizationListItems.Models;

namespace VirtualizationListItems.ViewModels
{
    class ThumbnailListViewModel : BindableBase
    {
        private readonly ImageSources ImageSources = ImageSources.Instance;

        private ReadOnlyObservableCollection<ThubnailVModel> _Thumbnails;
        public ReadOnlyObservableCollection<ThubnailVModel> Thumbnails
        {
            get => _Thumbnails;
            set => SetProperty(ref _Thumbnails, value);
        }

        public ThumbnailListViewModel()
        {
            // ObservableCollectionとReadOnlyObservableCollectionの同期☆
            var source = ImageSources.Sources;

            var proxy = new ObservableCollection<ThubnailVModel>();
            foreach (var v in source.Select((item, index) => (item, index)))
            {
                //v.item.PropertyChanged += ThumbnailItemPropertyChanged;
                proxy.Insert(v.index, new ThubnailVModel(v.item));
            }

            Thumbnails = new ReadOnlyObservableCollection<ThubnailVModel>(proxy);

            var collectionChanged = source.CollectionChangedAsObservable();
            collectionChanged.Where(e => e.Action == NotifyCollectionChangedAction.Add)
                .Select(e => new { Index = e.NewStartingIndex, Value = e.NewItems.Cast<ImageSource>().First() })
                .Subscribe(v =>
                {
                    //v.Value.PropertyChanged += ThumbnailItemPropertyChanged;
                    proxy.Insert(v.Index, new ThubnailVModel(v.Value));
                });
            collectionChanged.Where(e => e.Action == NotifyCollectionChangedAction.Remove)
                .Select(e => new { Index = e.OldStartingIndex, Value = e.OldItems.Cast<ImageSource>().First() })
                .Subscribe(v =>
                {
                    //v.Value.PropertyChanged -= ThumbnailItemPropertyChanged;
                    proxy.RemoveAt(v.Index);
                });

            // Clear()なら以下が来るけど、PropertyChanged()の解除ができないので使用しない
            //collectionChanged.Where(e => e.Action == NotifyCollectionChangedAction.Reset)

        }

    }

    class ThubnailVModel : BindableBase
    {
        public BitmapSource Image { get; }
        public string Filename { get; }

        public ThubnailVModel(ImageSource source)
        {
            Image = source.Thumbnail;
            Filename = source.Filename;
        }
    }

}
