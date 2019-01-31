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
        private readonly IRegionManager _regionManager;

        private string _text = "Jonathan,Joseph,Jotaro";
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public DelegateCommand<string> UpdateCommand { get; }

        public DelegateCommand<string> NavigateCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            UpdateCommand = new DelegateCommand<string>(text =>
            {
                if (string.IsNullOrEmpty(text)) return;
                var sep = text.Split(',');
                var targetViewNames = new[]
                {
                    "TabItemSingle", "TabItemDouble"
                };
                int count = Math.Min(sep.Length, targetViewNames.Length);

                var parameters = new NavigationParameters();
                for (int i = 0; i < count; i++)
                {
                    parameters.Add($"image{i}", sep[i]);
                }
                _regionManager.RequestNavigate("TabContentRegion", targetViewNames[count - 1], parameters);
            });

            NavigateCommand = new DelegateCommand<string>(x => _regionManager.RequestNavigate("TabContentRegion", x));

        }

    }
}
