using MetroWindow.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace MetroWindow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            /*
             * 以下で例外が発生するので下記対応で回避した(本家githubのsampleでも同現象が発生してた)
             *   例外：マネージド デバッグ アシスタント 'BindingFailure' : '表示名 'mscorlib.XmlSerializers' を伴うアセンブリを～
             *   対応：例外設定 → Managed Debugging Assistants → BindingFailure を OFF (根本解決になってない)
             */
            base.OnStartup(e);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
