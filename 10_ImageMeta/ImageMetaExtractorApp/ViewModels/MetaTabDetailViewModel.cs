using ImageMetaExtractorApp.Common;
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
using System.Collections.Generic;
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

        // メタ情報をまとめたクラス(Exif, MNoteなど)
        public ReactiveProperty<MetaItemGroup> MetaItemGroup { get; } =
            new ReactiveProperty<MetaItemGroup>(mode: ReactivePropertyMode.DistinctUntilChanged);

        // タブ名
        public ReadOnlyReactiveProperty<string> TabName { get; }

        // GridViewColumnのUnit表示フラグ(お気に入りタブだけ表示させたいので切り替える)
        public BooleanNotifier IsShowGridViewColumn { get; } = new BooleanNotifier();

        // MetaItemのフィルタ文字列
        public ReactiveProperty<string> FilterPattern { get; } = new ReactiveProperty<string>();

        // MetaItemのフィルタ文字列の削除コマンド
        public ReactiveCommand ClearFilterPatternCommand { get; }

        //お気に入りフィルタフラグ
        public ReactiveProperty<bool> IsFilterFavorite { get; } = new ReactiveProperty<bool>();

        //お気に入りフィルタ切り替えコマンド
        public DelegateCommand SwitchFavoriteFilterCommand { get; }

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
                .CombineLatest(FilterPattern, IsFilterFavorite,
                    (MetaGroup, Pattern, IsFav) => (MetaGroup, Pattern, IsFav))
                .Subscribe(x => UpdateFilterMetaItems(x.MetaGroup.Items, GetFilterPredicate(x.Pattern, x.IsFav)));

            // MetaItemのフィルタ文字列の削除
            ClearFilterPatternCommand = FilterPattern
                .Select(x => !string.IsNullOrEmpty(x))
                .ToReactiveCommand();

            ClearFilterPatternCommand.Subscribe(x_ => FilterPattern.Value = "");

            // お気に入りのみ表示
            SwitchFavoriteFilterCommand = new DelegateCommand(() => IsFilterFavorite.Value = !IsFilterFavorite.Value);

            // カラム選択で色付け
            SelectedItem.Subscribe(x => x?.SwitchMark());

            IsActiveChanged += ViewIsActiveChanged;
        }

        #region MetaFilter

        // フィルタ条件の取得
        private static Predicate<object> GetFilterPredicate(string pattern, bool isFav)
        {
            var predicates = new List<Predicate<object>>();

            // 指定文字列
            if (!string.IsNullOrEmpty(pattern))
                predicates.Add(obj => (obj as MetaItem).Key.Contains(pattern));

            // お気に入り
            if (isFav)
                predicates.Add(obj => (obj as MetaItem).IsMarking);

            if (predicates.Any())
                return PredicateExtensions.And(predicates.ToArray());

            return null;    // フィルタなし
        }

        // MetaItemのフィルタリング
        private void UpdateFilterMetaItems()
        {
            var metas = MetaItemGroup.Value?.Items;
            if (metas is null) return;
            UpdateFilterMetaItems(metas, GetFilterPredicate(FilterPattern.Value, IsFilterFavorite.Value));
        }

        // MetaItemのフィルタリング https://blog.okazuki.jp/entry/2014/10/29/220236
        private static void UpdateFilterMetaItems(ObservableCollection<MetaItem> collection, Predicate<object> filter)
        {
            if (collection is null) return;
            var collectionView = CollectionViewSource.GetDefaultView(collection);
            collectionView.Filter = filter;
        }

        #endregion

        #region INavigationAware

        public static NavigationParameters GetNavigationParameters(MetaItemGroup group) =>
            new NavigationParameters
                {
                    { MetaItemGroupKey, group },
                };

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[MetaItemGroupKey] is MetaItemGroup group)
            {
                MetaItemGroup.Value = group;

                // お気に入りタブなら所属名のカラムに幅を設ける(デフォで表示する)
                if (ImageMetasWithAll.IsAllGroup(group)) IsShowGridViewColumn.TurnOn();
            }
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
            //Debug.WriteLine($"ViewIsActiveChanged({e2.Value}) : {TabName.Value}");

            // 変化時はフィルタを更新(ALLがいると外部からお気に入り状態が変化するので)
            if (e2.Value)
            {
                UpdateFilterMetaItems();
            }
        }

        #endregion

    }
}
