using Prism.Ioc;
using Prism.Regions;
using SwitchContext.Common;
using System.Windows.Controls;

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
            for (int i = 0; i < count; i++)
            {
                var view = container.Resolve<ImagePanel>();
                view.SetContentIndex(i);

                regionManager.RegisterViewWithRegion(
                    RegionNames.GetImageContentRegionName(count, i),
                    () => view);
            }
        }
    }
}
