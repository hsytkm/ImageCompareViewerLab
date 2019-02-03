using Prism.Ioc;
using Prism.Regions;
using System.Windows;

namespace SwitchContext.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();

            regionManager.RegisterViewWithRegion("TabContentRegion", typeof(SingleImageTabItem));
            regionManager.RegisterViewWithRegion("TabContentRegion", typeof(DoubleImageTabItem));
            regionManager.RegisterViewWithRegion("TabContentRegion", typeof(TripleImageTabItem));
        }
    }
}
