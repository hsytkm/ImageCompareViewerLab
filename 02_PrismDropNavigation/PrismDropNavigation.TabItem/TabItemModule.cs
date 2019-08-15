using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using PrismDropNavigation.TabItem.ViewModels;
using PrismDropNavigation.TabItem.Views;
using System;
using System.Collections.Generic;
using System.Windows;

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

            for (int i = 0; i < TabItemTypes.Count; i++)
            {
                var tab = containerProvider.Resolve(TabItemTypes[i]);
                if (tab is FrameworkElement fe)
                {
                    (fe.DataContext as TabItemViewModelBase).SetIndex(i + 1);
                }
                region.Add(tab);
            }

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}