using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using PrismDispose.Module1.Views;
using System.Linq;
using System.Windows.Controls;

namespace PrismDispose.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private readonly IContainerExtension _container;
        private readonly IRegionManager _regionManager;

        public DelegateCommand AddCommand { get; }

        public DelegateCommand ClosedCommand { get; }

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;

            AddCommand = new DelegateCommand(() => AddModule<ViewA>("ContentRegion"));

            ClosedCommand = new DelegateCommand(() =>
                _regionManager.Regions["ContentRegion"].RemoveAll());
        }

        // 指定リージョンにモジュールを追加
        private void AddModule<T>(string regionName) where T : UserControl
        {
            var name = typeof(T).Name;
            var viewTarget = _regionManager.Regions[regionName].Views
                .FirstOrDefault<dynamic>(v => v.ViewName == name);

            if (viewTarget == null)
            {
                var view = _container.Resolve<T>();
                _regionManager.Regions[regionName].Add(view, name);
            }
        }

    }
}
