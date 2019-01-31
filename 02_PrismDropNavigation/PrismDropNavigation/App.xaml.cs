using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using PrismDropNavigation.Views;
using System.Windows;

namespace PrismDropNavigation
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
        
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<TabItem.TabItemModule>();
        }

    }
}
