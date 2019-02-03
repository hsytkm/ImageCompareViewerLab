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

            var views = Enumerable.Range(0, 3)
                .Select(x =>
                {
                    var v = container.Resolve<ImagePanel>();
                    SetImageIndex(v, x);
                    return v;
                });

            views.ForEach(v => regionManager.RegisterViewWithRegion("Image3ContentRegion", () => v));
        }
    }
}
