using Prism.Ioc;
using Prism.Regions;
using SwitchContext.Common;
using System.Windows.Controls;

namespace SwitchContext.Views
{
    /// <summary>
    /// SingleImageTabItem.xaml の相互作用ロジック
    /// </summary>
    public partial class SingleImageTabItem : UserControl
    {
        public SingleImageTabItem(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();

            // 以下で動くが他画像に合わせる
            //regionManager.RegisterViewWithRegion(RegionNames.ImageContentRegion1_0, typeof(ImagePanel));

            int count = 1;
            for (int i = 0; i < count; i++)
            {
                regionManager.RegisterViewWithRegion(
                    RegionNames.GetImageContentRegionName(count, i),
                    () => container.Resolve<ImagePanel>());
            }
        }
    }
}
