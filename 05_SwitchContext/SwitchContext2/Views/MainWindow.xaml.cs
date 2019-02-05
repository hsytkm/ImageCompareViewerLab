using Prism.Ioc;
using Prism.Regions;
using SwitchContext.Common;
using System.Windows;
using ThosoImage.Extensions;

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

            new[]
            {
                typeof(SingleImageTabItem),
                typeof(DoubleImageTabItem),
                typeof(TripleImageTabItem),
            }
            .ForEach(x => regionManager.RegisterViewWithRegion(RegionNames.TabContentRegion, x));
        }
    }
}
