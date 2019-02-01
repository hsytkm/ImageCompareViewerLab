using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using PrismDispose.Module1.Views;

namespace PrismDispose.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private readonly IContainerExtension _container;
        private readonly IRegionManager _regionManager;

        public DelegateCommand NavigateCommand { get; }

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;

            NavigateCommand = new DelegateCommand(() =>
            {
                //_regionManager.RegisterViewWithRegion("ContentRegion", typeof(ViewA));
                var view = _container.Resolve<ViewA>();
                _regionManager.Regions["ContentRegion"].Add(view, nameof(ViewA));
            });

        }

    }
}
