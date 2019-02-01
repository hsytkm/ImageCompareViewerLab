using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using PrismDispose.Views;
using PrismDispose.Views.RegionBehaviors;
using System.Windows;

namespace PrismDispose
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry) { }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) { }

        protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
        {
            regionBehaviors.AddIfMissing(DisposeBehavior.Key, typeof(DisposeBehavior));
            base.ConfigureDefaultRegionBehaviors(regionBehaviors);
        }

    }
}
