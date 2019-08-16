using ImageMetaExtractorApp.Models;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
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
        public ReactiveProperty<MetaItemGroup> MetaItemGroup { get; } =
            new ReactiveProperty<MetaItemGroup>(mode: ReactivePropertyMode.DistinctUntilChanged);

        // お気に入り用メタ情報クラス
        private ImageMetasFav _imageMetas;

        // タブ名
        public ReadOnlyReactiveProperty<string> TabName { get; }

        // GridViewColumnのUnit表示フラグ(お気に入りタブだけ表示させたいので切り替える)
        public BooleanNotifier IsShowGridViewColumn { get; } = new BooleanNotifier();

        // MetaItemのフィルタ文字列
        public ReactiveProperty<string> FilterPattern { get; } = new ReactiveProperty<string>();

        // MetaItemのフィルタ文字列の削除コマンド
        public DelegateCommand ClearFilterPatternCommand { get; }

        // View選択項目(同項目の選択に反応させるためDistinctUntilChangedを指定しない)
        public ReactiveProperty<MetaItem> SelectedItem { get; } =
            new ReactiveProperty<MetaItem>(mode: ReactivePropertyMode.None);

        public MetaTabDetailViewModel()
        {
            // タブ名
            TabName = MetaItemGroup
                .Select(x => x.Name)
                .ToReadOnlyReactiveProperty();

            // MetaItemの文字列フィルタリング
            MetaItemGroup
                .CombineLatest(FilterPattern, (MetaGroup, Pattern) => (MetaGroup, Pattern))
                .Subscribe(x => FilterMetaItems(x.MetaGroup.Items, x.Pattern));

            // MetaItemのフィルタ文字列の削除
            ClearFilterPatternCommand = new DelegateCommand(() => FilterPattern.Value = "");

            // カラム選択で色付け
            SelectedItem.Subscribe(x => x?.SwitchMark());

            IsActiveChanged += ViewIsActiveChanged;
        }

        // MetaItemのフィルタリング https://blog.okazuki.jp/entry/2014/10/29/220236
        private static void FilterMetaItems(ObservableCollection<MetaItem> collection, string pattern)
        {
            if (collection is null) return;

            var collectionView = CollectionViewSource.GetDefaultView(collection);
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
                MetaItemGroup.Value = group;

                // お気に入りタブなら所属名のカラムに幅を設ける(デフォで表示する)
                if (ImageMetasFav.IsFavGroup(group)) IsShowGridViewColumn.TurnOn();
            }

            if (navigationContext.Parameters[ImageMetasKey] is ImageMetasFav imageMetas)
                _imageMetas = imageMetas;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            var fieldGroup = MetaItemGroup.Value;
            if (navigationContext.Parameters[MetaItemGroupKey] is MetaItemGroup group)
                return fieldGroup != null && fieldGroup.Name == group.Name;
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }

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

            var metaItemGroup = MetaItemGroup.Value;
            //Debug.WriteLine($"ViewIsActiveChanged({e2}) : {metaItemGroup.Name}");

            if (!e2.Value)
            {
                _imageMetas.AddFavMetaItem(metaItemGroup);
            }
        }

        #endregion

    }
}
