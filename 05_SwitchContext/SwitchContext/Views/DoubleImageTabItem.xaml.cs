using Prism.Ioc;
using Prism.Regions;
using System.Windows.Controls;

namespace SwitchContext.Views
{
    /// <summary>
    /// DoubleImageTabItem.xaml の相互作用ロジック
    /// </summary>
    public partial class DoubleImageTabItem : UserControl
    {
        public DoubleImageTabItem(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();

            regionManager.RegisterViewWithRegion("Image2ContentRegion", typeof(ImagePanel));
            regionManager.RegisterViewWithRegion("Image2ContentRegion", typeof(ImagePanel));
        }
    }
}
