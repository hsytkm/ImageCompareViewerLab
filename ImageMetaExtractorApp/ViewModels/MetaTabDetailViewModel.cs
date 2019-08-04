using ImageMetaExtractorApp.Models;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ImageMetaExtractorApp.ViewModels
{
    class MetaTabDetailViewModel : BindableBase, INavigationAware, IActiveAware
    {
        // NavigationContextのKey
        private static readonly string MetaItemGroupKey = nameof(MetaItemGroupKey);
        private static readonly string ImageMetasKey = nameof(ImageMetasKey);

        // メタ情報をまとめたクラス(Exif, MNoteなど)
        public MetaItemGroup MetaItemGroup
        {
            get => _MetaItemGroup;
            private set => SetProperty(ref _MetaItemGroup, value);
        }
        private MetaItemGroup _MetaItemGroup;

        // お気に入り用
        private ImageMetasFav _ImageMetas;

        // View選択項目(同項目の選択に反応させるためDistinctUntilChangedｗ指定しない)
        public ReactiveProperty<MetaItem> SelectedItem { get; } =
            new ReactiveProperty<MetaItem>(mode: ReactivePropertyMode.None);

        public MetaTabDetailViewModel()
        {
            // カラム選択で色付け
            SelectedItem.Subscribe(x => x?.SwitchMark());

            IsActiveChanged += ViewIsActiveChanged;
        }

        #region INavigationAware

        public static NavigationParameters GetNavigationParameters(MetaItemGroup group, ImageMetasFav imageMetas) =>
            new NavigationParameters
                {
                    { MetaItemGroupKey, group },
                    { ImageMetasKey, imageMetas },
                };

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[MetaItemGroupKey] is MetaItemGroup group)
                MetaItemGroup = group;

            if (navigationContext.Parameters[ImageMetasKey] is ImageMetasFav imageMetas)
                _ImageMetas = imageMetas;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[MetaItemGroupKey] is MetaItemGroup group)
                return MetaItemGroup != null && MetaItemGroup.Name == group.Name;
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion

        #region IActiveAware

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (SetProperty(ref _isActive, value))
                    IsActiveChanged?.Invoke(this, new DataEventArgs<bool>(value));
            }
        }
        private bool _isActive;

        public event EventHandler IsActiveChanged;

        // アクティブ状態変化時の処理
        private void ViewIsActiveChanged(object sender, EventArgs e)
        {
            if (!(e is DataEventArgs<bool> e2)) return;
            if (e2.Value)
            {
                //Console.WriteLine($"Active : {MetaItemGroup?.Name}");
            }
            else
            {
                //Console.WriteLine($"Inactive : {MetaItemGroup?.Name}");
                _ImageMetas.AddFavMetaItem(MetaItemGroup);
            }
        }

        #endregion

    }
}
