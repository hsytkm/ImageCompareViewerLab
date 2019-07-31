using ImageMetaExtractorApp.Models;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ImageMetaExtractorApp.ViewModels
{
    class MetaTabDetailViewModel : BindableBase, INavigationAware
    {
        // NavigationContextのKey
        public static readonly string MetaItemGroupKey = "metaItemGroup";

        // メタ情報をまとめたクラス(Exif, MNoteなど)
        public MetaItemGroup MetaItemGroup
        {
            get => _MetaItemGroup;
            private set => SetProperty(ref _MetaItemGroup, value);
        }
        private MetaItemGroup _MetaItemGroup;

        // View選択項目(同項目の選択に反応させるためDistinctUntilChangedｗ指定しない)
        public ReactiveProperty<MetaItem> SelectedItem { get; } =
            new ReactiveProperty<MetaItem>(mode: ReactivePropertyMode.None);

        public MetaTabDetailViewModel()
        {
            // カラム選択で色付け
            SelectedItem.Where(x => x != null).Subscribe(x => x.SwitchMark());
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[MetaItemGroupKey] is MetaItemGroup group)
                MetaItemGroup = group;
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

    }
}
