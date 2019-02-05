using Prism.Regions;
using System.Windows.Controls;
using SwitchContext.Common;

namespace SwitchContext.Views
{
    /// <summary>
    /// SingleImageTabItem.xaml の相互作用ロジック
    /// </summary>
    public partial class SingleImageTabItem : UserControl
    {
        public SingleImageTabItem(IRegionManager regionManager)
        {
            InitializeComponent();

            regionManager.RegisterViewWithRegion(RegionNames.ImageContentRegion1, typeof(ImagePanel));
        }
    }
}
