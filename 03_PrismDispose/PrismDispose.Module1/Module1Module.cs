using PrismDispose.Module1.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace PrismDispose.Module1
{
    public class Module1Module : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(ViewA));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}