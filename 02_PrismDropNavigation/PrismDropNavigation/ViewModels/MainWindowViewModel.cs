using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismDropNavigation.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private static readonly int InitializeTabIndex = 1;     //0=Tab1

        private readonly IRegionManager _regionManager;

        private string _text = "Jonathan,Joseph,Jotaro";
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        // TabItemsのView名(モジュール側と名前を揃えないとダメ)
        private static readonly string[] TabItemViewNames = new[]
        {
            "TabItemSingle", "TabItemDouble"
        };

        public DelegateCommand InitializeCommand { get; }

        public DelegateCommand<string> UpdateCommand { get; }

        public DelegateCommand<string> NavigateCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            // 起動時の初期タブ位置を変更
            InitializeCommand = new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate("TabContentRegion", TabItemViewNames[InitializeTabIndex]);
            });

            UpdateCommand = new DelegateCommand<string>(text =>
            {
                if (string.IsNullOrEmpty(text)) return;
                var sep = text.Split(',');
                int count = Math.Min(sep.Length, TabItemViewNames.Length);

                var parameters = new NavigationParameters();
                for (int i = 0; i < count; i++)
                {
                    parameters.Add($"image{i}", sep[i]);
                }
                _regionManager.RequestNavigate("TabContentRegion", TabItemViewNames[count - 1], parameters);
            });

            NavigateCommand = new DelegateCommand<string>(x => _regionManager.RequestNavigate("TabContentRegion", x));

        }

    }
}
