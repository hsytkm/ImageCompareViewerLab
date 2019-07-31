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

        private readonly ImageMetas ImageMetas = new ImageMetas();

        private readonly IRegionManager _regionManager;

        public DelegateCommand AddTab1Command { get; }
        public DelegateCommand AddTab2Command { get; }
        public DelegateCommand ClearAllMarksCommand { get; }
        
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

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            AddTab1Command = new DelegateCommand(AddTab1);
            AddTab2Command = new DelegateCommand(AddTab2);
            ClearAllMarksCommand = new DelegateCommand(ClearAllMarks);
        }

        private void ClearAllMarks() => ImageMetas.ClearAllMarking();

        private void AddTab1()
        {
            ImageMetas.Load(ImageSource1);
            AddTab();
        }

        private void AddTab2()
        {
            ImageMetas.Load(ImageSource2);
            AddTab();
        }

        private void AddTab()
        {
            // Region追加時に選択が更新されてしまうので先にバフっとく
            var resumeTabTitle = TabControlSelectedTitle;
            var regionName = MetaTabDetailsRegionName;

            foreach (var metaItemGroup in ImageMetas.MetaItemGroups.Where(x => x != null))
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
