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

            var view1 = container.Resolve<ImagePanel>();
            SetImageIndex(view1, 0);
            regionManager.RegisterViewWithRegion("Image2ContentRegion", () => view1);

            var view2 = container.Resolve<ImagePanel>();
            SetImageIndex(view2, 1);
            regionManager.RegisterViewWithRegion("Image2ContentRegion", () => view2);

            // NG:以下では登録できるがViewModelに引数を渡せない
            //regionManager.RegisterViewWithRegion("Image2ContentRegion", typeof(ImagePanel));

            // NG:以下で登録しようとするとRegionが見つからないと言われる謎
            //regionManager.AddToRegion("Image2ContentRegion", container.Resolve<ImagePanel>());

            // NG:以下で登録しようとするとRegionが見つからないと言われる謎
            //regionManager.Regions["Image2ContentRegion"].Add(container.Resolve<ImagePanel>());

            void SetImageIndex(dynamic fe, int index) => fe.DataContext.Index = index;
        }
    }
}
