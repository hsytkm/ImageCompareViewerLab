using Prism.Regions;
using System.Windows.Controls;

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

            regionManager.RegisterViewWithRegion("Image1ContentRegion", typeof(ImagePanel));
        }
    }
}
