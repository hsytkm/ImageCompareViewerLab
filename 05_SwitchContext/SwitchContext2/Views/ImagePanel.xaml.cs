using SwitchContext.ViewModels;
using System.Windows.Controls;

namespace SwitchContext.Views
{
    /// <summary>
    /// ImagePanel.xaml の相互作用ロジック
    /// </summary>
    public partial class ImagePanel : UserControl
    {
        public ImagePanel()
        {
            InitializeComponent();
        }

        public void SetContentIndex(int index)
        {
            if (DataContext is ImagePanelViewModel vmodel)
            {
                vmodel.ContentIndex = index;
            }
        }

    }
}
