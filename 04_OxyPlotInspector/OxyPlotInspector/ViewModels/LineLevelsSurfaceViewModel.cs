using OxyPlotInspector.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace OxyPlotInspector.ViewModels
{
    class LineLevelsSurfaceViewModel : BindableBase
    {
        public DelegateCommand LazyViewLoadCommand { get; }

        public LineLevelsSurfaceViewModel(IRegionManager regionManager)
        {
            LazyViewLoadCommand = new DelegateCommand(() =>
                regionManager.RegisterViewWithRegion("LineLevelsRegion", typeof(LineLevels)));
        }
    }
}
