using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using SwitchContext.Views;
using System.Windows;

namespace SwitchContext
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
        }

    }
}
