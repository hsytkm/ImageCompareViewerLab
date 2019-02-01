using OxyPlotInspector.Views;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System.Linq;
using System.Windows.Controls;

namespace OxyPlotInspector.ViewModels
{
    class MainImageViewModel : BindableBase
    {
        private readonly IContainerExtension _container;
        private readonly IRegionManager _regionManager;

        public MainImageViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

    }
}
