using PrismDropNavigation.TabItem.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using PrismDropNavigation.TabItem.ViewModels;
using System.Windows;

namespace PrismDropNavigation.TabItem
{
    public class TabItemModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            var region = regionManager.Regions["TabContentRegion"];

            var tab1 = containerProvider.Resolve<TabItemSingle>();
            SetTabTitle(tab1, nameof(tab1));
            region.Add(tab1);

            var tab2 = containerProvider.Resolve<TabItemDouble>();
            SetTabTitle(tab2, nameof(tab2));
            region.Add(tab2);

            void SetTabTitle(FrameworkElement fe, string title) =>
                (fe.DataContext as ITabItemViewModel).Title = title;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}