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
        /* 以下で例外が発生するので下記対応で回避した (根本解決になってない)
         * 本家githubのsampleでも同現象が発生してたので…
         *   例外：マネージド デバッグ アシスタント 'BindingFailure' : '表示名 'mscorlib.XmlSerializers' を伴うアセンブリを～
         *   対応：例外設定 → Common Language Runtime Exceptions → System.IO.FileNotFoundException を OFF
         *   対応：例外設定 → Managed Debugging Assistants → BindingFailure を OFF
         */
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    base.OnStartup(e);
        //}

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
