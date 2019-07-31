using ImageMetaExtractorApp.Models;
using ImageMetaExtractorApp.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace ImageMetaExtractorApp.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private const string MetaTabDetailsRegionName = "MetaTabDetailsRegion";
        private const string ImageSource1 = @"C:\data\Image1.JPG";
        private const string ImageSource2 = @"C:\data\Image2.JPG";

        private readonly ImageMetas ImageMetas1;
        private readonly ImageMetas ImageMetas2;

        private readonly IRegionManager _regionManager;

        public DelegateCommand AddTab1Command { get; }
        public DelegateCommand AddTab2Command { get; }

        // 選択中のタブ要素(View)
        public ReactiveProperty<object> TabControlSelectedItem { get; } = new ReactiveProperty<object>();

        // 選択中のタブ名
        private string _SelectedTabTitle;

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            ImageMetas1 = new ImageMetas(ImageSource1);
            ImageMetas2 = new ImageMetas(ImageSource2);

            AddTab1Command = new DelegateCommand(AddTab1);
            AddTab2Command = new DelegateCommand(AddTab2);

            // タブの選択更新時にタブ名を取得する
            TabControlSelectedItem
                .Select(x => (x as ContentControl)?.DataContext)
                .Select(x => (x as MetaTabDetailViewModel)?.MetaItemGroup?.Name)
                .Where(x => x != null)
                .Subscribe(x => _SelectedTabTitle = x);
        }

        public void AddTab1() => AddTab(ImageMetas1);
        public void AddTab2() => AddTab(ImageMetas2);

        private void AddTab(ImageMetas imageMetas)
        {
            // Region追加時に選択が更新されてしまうので先にバフっとく
            var resumeTabTitle = _SelectedTabTitle;
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

        private void ActivateRegion(string regionName, string tabTitle)
        {
            var views = _regionManager.Regions[regionName].Views;
            if (!views.Any()) return;

            var target = views.FirstOrDefault(x => GetTabTitle(x) == tabTitle);
            if (target != null)
                _regionManager.Regions[regionName].Activate(target);
            else
                ActivateRegion(regionName, index: 0);  // ヒットしなければ先頭にする
        }

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

        private static string GetTabTitle(object obj)
        {
            if (obj is ContentControl cc)
                if (cc.DataContext is MetaTabDetailViewModel vm)
                    return vm.MetaItemGroup?.Name;
            return null;
        }

    }

}
