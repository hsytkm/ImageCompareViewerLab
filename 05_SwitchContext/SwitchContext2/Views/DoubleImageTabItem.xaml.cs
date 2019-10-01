using Prism.Ioc;
using Prism.Regions;
using SwitchContext.Common;
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

            int count = 2;
            for (int i = 0; i < count; i++)
            {
                var view = container.Resolve<ImagePanel>();
                view.SetContentIndex(i);

                regionManager.RegisterViewWithRegion(
                    RegionNames.GetImageContentRegionName(count, i),
                    () => view);
            }

            // NG:以下では登録できるがViewModelに引数を渡せない
            //regionManager.RegisterViewWithRegion("Image2ContentRegion", typeof(ImagePanel));

            // NG:以下で登録しようとするとRegionが見つからないと言われる謎
            //regionManager.AddToRegion("Image2ContentRegion", container.Resolve<ImagePanel>());

            // NG:以下で登録しようとするとRegionが見つからないと言われる謎
            //regionManager.Regions["Image2ContentRegion"].Add(container.Resolve<ImagePanel>());
        }
    }
}
