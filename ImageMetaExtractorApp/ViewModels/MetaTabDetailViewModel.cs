using ImageMetaExtractorApp.Models;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System;
using System.Reactive.Linq;
using System.Windows.Data;

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

        // お気に入り用メタ情報クラス
        private ImageMetasFav _ImageMetas;

        // GridViewColumnのUnit表示フラグ(お気に入りタブだけ表示させたいので切り替える)
        public bool IsShowGridViewColumnUnit
        {
            get => _IsShowGridViewColumnUnit;
            private set => SetProperty(ref _IsShowGridViewColumnUnit, value);
        }
        private bool _IsShowGridViewColumnUnit;

        // View選択項目(同項目の選択に反応させるためDistinctUntilChangedを指定しない)
        public ReactiveProperty<MetaItem> SelectedItem { get; } =
            new ReactiveProperty<MetaItem>(mode: ReactivePropertyMode.None);

        // MetaItemのフィルタ文字列
        public ReactiveProperty<string> FilterPattern { get; } = new ReactiveProperty<string>();

        // MetaItemのフィルタ文字列の削除コマンド
        public DelegateCommand ClearFilterPatternCommand { get; }

        public MetaTabDetailViewModel()
        {
            // カラム選択で色付け
            SelectedItem.Subscribe(x => x?.SwitchMark());

            IsActiveChanged += ViewIsActiveChanged;

            // MetaItemのフィルタリング
            FilterPattern.Subscribe(pat => FilterMetaItems(pat));

            // MetaItemのフィルタ文字列の削除
            ClearFilterPatternCommand = new DelegateCommand(() => FilterPattern.Value = "");

        }

        // MetaItemのフィルタリング https://blog.okazuki.jp/entry/2014/10/29/220236
        private void FilterMetaItems(string pattern)
        {
            var itemsSource = MetaItemGroup?.Items;
            if (itemsSource is null) return;

            var collectionView = CollectionViewSource.GetDefaultView(itemsSource);
            if (string.IsNullOrEmpty(pattern))
            {
                collectionView.Filter = x => true;
            }
            else
            {
                collectionView.Filter = x => (x as MetaItem).Key.Contains(pattern);
            }
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
            {
                MetaItemGroup = group;

                // お気に入りタブなら所属名のカラムに幅を設ける(デフォで表示する)
                IsShowGridViewColumnUnit = ImageMetasFav.IsFavGroup(group);
            }

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
