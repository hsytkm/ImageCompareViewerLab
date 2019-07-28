using ImageMetaExtractorApp.Models;
using Prism.Mvvm;
using Prism.Regions;

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

        public MetaTabDetailViewModel() { }

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
