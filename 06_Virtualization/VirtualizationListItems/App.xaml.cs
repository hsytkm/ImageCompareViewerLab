using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using VirtualizationListItems.Models;
using VirtualizationListItems.ViewModels;
using VirtualizationListItems.Views;

namespace VirtualizationListItems
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ImageSources>();
            containerRegistry.RegisterDialog<ConfirmDialog, ConfirmDialogViewModel>();
        }

    }
}
