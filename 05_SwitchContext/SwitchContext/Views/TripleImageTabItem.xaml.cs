using Prism.Ioc;
using Prism.Regions;
using System.Linq;
using System.Windows.Controls;
using ThosoImage.Extensions;

namespace SwitchContext.Views
{
    /// <summary>
    /// TripleImageTabItem.xaml の相互作用ロジック
    /// </summary>
    public partial class TripleImageTabItem : UserControl
    {
        public TripleImageTabItem(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();

            int count = 3;
            string regionName = $"Image{count}ContentRegion";
            for (int i = 0; i < count; i++)
            {
                regionManager.RegisterViewWithRegion(regionName,
                    () => container.Resolve<ImagePanel>());
            }
        }
    }
}
