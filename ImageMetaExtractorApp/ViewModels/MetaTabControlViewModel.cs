using ImageMetaExtractorApp.Models;
using ImageMetaExtractorApp.Views;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings.Extensions;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace ImageMetaExtractorApp.ViewModels
{
    class MetaTabControlViewModel : BindableBase
    {
        private const string MetaTabDetailsRegionName = "MetaTabDetailsRegion";

#if false
        // xaml
        SelectedItem="{Binding TabControlSelectedItem.Value, Mode=OneWayToSource}"

        // 選択中のタブ要素(View)
        public ReactiveProperty<object> TabControlSelectedItem { get; } = new ReactiveProperty<object>();

        // タブの選択更新時にタブ名を取得する
        TabControlSelectedItem
            .Select(x => (x as ContentControl)?.DataContext)
            .Select(x => (x as MetaTabDetailViewModel)?.MetaItemGroup?.Name)
            .Where(x => x != null)
            .Subscribe(x => _SelectedTabTitle = x);
#endif
        // Viewで選択中のタブ名(OneWayToSource)
        public string TabControlSelectedTitle { get; set; }

        private readonly IRegionManager _regionManager;
        private readonly ModelMaster _modelMaster;

        public MetaTabControlViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _modelMaster = container.Resolve<ModelMaster>();

            _modelMaster.ObserveProperty(x => x.ImageMetas).Subscribe(x => AddTab(x));
        }

        // メタ情報クラスからView用のTabを読み出し
        private void AddTab(ImageMetas imageMetas)
        {
            if (imageMetas is null) return;

            // Region追加時に選択が更新されてしまうので先にバフっとく
            var resumeTabTitle = TabControlSelectedTitle;
            var regionName = MetaTabDetailsRegionName;

            foreach (var metaItemGroup in imageMetas.MetaItemGroups.Where(x => x != null))
            {
                var parameters = new NavigationParameters
                {
                    { MetaTabDetailViewModel.MetaItemGroupKey, metaItemGroup }
                };
                _regionManager.RequestNavigate(regionName, nameof(MetaTabDetail), parameters);
            }
            // 読み込み後の表示位置を指定
            ActivateRegion(regionName, resumeTabTitle);
        }

        // 引数リージョンのタブ名を表示する
        private void ActivateRegion(string regionName, string tabTitle)
        {
            var views = _regionManager.Regions[regionName].Views;
            var target = views.FirstOrDefault(x => GetTabTitle(x) == tabTitle);
            if (target != null)
                _regionManager.Regions[regionName].Activate(target);
            else
                ActivateRegion(regionName, index: 0);  // ヒットしなければ先頭にする
        }

        // 引数リージョンの番号を表示する
        private void ActivateRegion(string regionName, int index)
        {
            var views = _regionManager.Regions[regionName].Views;
            int count = views.Count();
            if (count == 0) return;

            if (index < 0) index = 0;
            else if (index > count) index = count - 1;

            var target = views.ElementAt(index);
            _regionManager.Regions[regionName].Activate(target);
        }

        // クラスからタブ名を取得
        private static string GetTabTitle(object obj)
        {
            if (obj is ContentControl cc)
                if (cc.DataContext is MetaTabDetailViewModel vm)
                    return vm.MetaItemGroup?.Name;
            return null;
        }

    }
}
