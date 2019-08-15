using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using PrismDropNavigation.TabItem;
using PrismDropNavigation.TabItem.ViewModels;
using System;

namespace PrismDropNavigation.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private static readonly string TabContentRegionName = "TabContentRegion";
        private static readonly int InitializeTabIndex = 1;     //0=Tab1

        private readonly IRegionManager _regionManager;

        private string _text = "Jonathan,Joseph,Jotaro";
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public DelegateCommand InitializeCommand { get; }

        public DelegateCommand<string> UpdateCommand { get; }

        //public DelegateCommand<string> NavigateCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            var regionName = TabContentRegionName;

            // 起動時の初期タブ位置を変更
            InitializeCommand = new DelegateCommand(() =>
            {
                var viewName = GetTabItemViewName(InitializeTabIndex);
                _regionManager.RequestNavigate(regionName, viewName);
            });

            UpdateCommand = new DelegateCommand<string>(text =>
            {
                if (string.IsNullOrEmpty(text)) return;
                var sep = text.Split(',');
                int count = Math.Min(sep.Length, TabItemModule.TabItemTypes.Count);
                var parameters = TabItemViewModelBase.GetNavigationParameters(sep);

                _regionManager.RequestNavigate(regionName, GetTabItemViewName(count - 1), parameters);
            });

            //NavigateCommand = new DelegateCommand<string>(x =>
            //    _regionManager.RequestNavigate(regionName, x));

        }

        // TabItemのView名を取得(◆Itemを個別クラスで定義しないと切り替える方法分からなかった…)
        private static string GetTabItemViewName(int index) =>
            TabItemModule.TabItemTypes[index].Name;

    }
}
