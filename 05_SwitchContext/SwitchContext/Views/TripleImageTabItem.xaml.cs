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

            void SetImageIndex(dynamic fe, int index) => fe.DataContext.Index = index;

            int count = 3;
            string regionName = $"Image{count}ContentRegion";

            Enumerable.Range(0, count)
                .Select(x =>
                {
                    var v = container.Resolve<ImagePanel>();
                    SetImageIndex(v, x);
                    return v;
                })
                .ForEach(v => regionManager.RegisterViewWithRegion(regionName, () => v));
        }
    }
}
