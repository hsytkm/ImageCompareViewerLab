using ImageMetaExtractorApp.Models;
using ImageMetaExtractorApp.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace ImageMetaExtractorApp
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
            containerRegistry.RegisterForNavigation<MetaTabDetail>();
            containerRegistry.RegisterSingleton<ModelMaster>();
        }
    }
}   
