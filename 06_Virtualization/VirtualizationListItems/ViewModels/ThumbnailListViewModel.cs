using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Media.Imaging;
using VirtualizationListItems.Models;

namespace VirtualizationListItems.ViewModels
{
    class ThumbnailListViewModel : BindableBase
    {
        public ReadOnlyObservableCollection<ThubnailVModel> Thumbnails { get; }

        public ReactiveProperty<ThubnailVModel> SelectedItem { get; } =
            new ReactiveProperty<ThubnailVModel>(mode: ReactivePropertyMode.DistinctUntilChanged);

        // スクロール変化時
        public ReactiveProperty<(double CenterRatio, double ViewportRatio)> ScrollChangedHorizontal { get; } =
            new ReactiveProperty<(double CenterRatio, double ViewportRatio)>(mode: ReactivePropertyMode.None);

        public ThumbnailListViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            var imageSources = container.Resolve<ImageSources>();

            // ObservableCollectionとReadOnlyObservableCollectionの同期☆
            var source = imageSources.Sources;

            var proxy = new ObservableCollection<ThubnailVModel>();

#if false   // いらん気がする。いつ呼ばれる？
            foreach (var v in source.Select((item, index) => (item, index)))
            {
                v.item.PropertyChanged += ThumbnailPropertyChanged;
                proxy.Insert(v.index, new ThubnailVModel(v.item));
            }
#endif

            Thumbnails = new ReadOnlyObservableCollection<ThubnailVModel>(proxy);

            var collectionChanged = source.CollectionChangedAsObservable();
            collectionChanged.Where(e => e.Action == NotifyCollectionChangedAction.Add)
                .Select(e => new { Index = e.NewStartingIndex, Value = e.NewItems.Cast<ImageSource>().First() })
                .Subscribe(v =>
                {
                    v.Value.PropertyChanged += ThumbnailPropertyChanged;
                    proxy.Insert(v.Index, new ThubnailVModel(v.Value));
                });
            collectionChanged.Where(e => e.Action == NotifyCollectionChangedAction.Remove)
                .Select(e => new { Index = e.OldStartingIndex, Value = e.OldItems.Cast<ImageSource>().First() })
                .Subscribe(v =>
                {
                    v.Value.PropertyChanged -= ThumbnailPropertyChanged;
                    proxy.RemoveAt(v.Index);
                });

            // Clear()なら以下が来るけど、PropertyChanged()の解除ができないので使用しない
            //collectionChanged.Where(e => e.Action == NotifyCollectionChangedAction.Reset)
            
            // VM→M通知(nullになったらnullを通知する。後で変化エッジを付けるため)
            SelectedItem.Subscribe(x => imageSources.SelectedImagePath = x?.FilePath);

            // M→VM通知
            imageSources.ObserveProperty(x => x.SelectedImagePath)
                .Subscribe(x => SelectedItem.Value = Thumbnails.FirstOrDefault(y => x == y.FilePath));

            ScrollChangedHorizontal
                .Subscribe(x => imageSources.UpdateThumbnail(x.CenterRatio, x.ViewportRatio));
        }

        /// <summary>
        /// Modelサムネイル画像変化イベント(Modelの値が変化したら、ViewModelに値を反映)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThumbnailPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is ImageSource imageSource)) return;

            if (e.PropertyName == nameof(imageSource.Thumbnail))
            {
                var vModel = Thumbnails.FirstOrDefault(x => x.FilePath == imageSource.FilePath);
                if (vModel != null) vModel.Image = imageSource.Thumbnail;
            }
        }
    }

    class ThubnailVModel : BindableBase
    {
        private BitmapSource _Image;
        public BitmapSource Image
        {
            get => _Image;
            set => SetProperty(ref _Image, value);
        }
        public string FilePath { get; }
        public string Filename { get; }

        public ThubnailVModel(ImageSource source)
        {
            Image = source.Thumbnail;
            FilePath = source.FilePath;
            Filename = source.Filename;
        }
    }

}
