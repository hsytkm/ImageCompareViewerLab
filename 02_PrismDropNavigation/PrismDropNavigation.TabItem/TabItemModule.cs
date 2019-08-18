using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using PrismDropNavigation.TabItem.Views;
using System;
using System.Collections.Generic;

namespace PrismDropNavigation.TabItem
{
    public class TabItemModule : IModule
    {
        public static IList<Type> TabItemTypes { get; } = new[]
        {
            typeof(TabItemSingle),
            typeof(TabItemDouble),
            typeof(TabItemTriple),
        };

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            var region = regionManager.Regions["TabContentRegion"];

            foreach (var type in TabItemTypes)
            {
                var tab = containerProvider.Resolve(type);
                region.Add(tab);
            }

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}