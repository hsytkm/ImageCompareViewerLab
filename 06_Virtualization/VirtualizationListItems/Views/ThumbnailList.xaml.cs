using System.Windows;
using System.Windows.Controls;

namespace VirtualizationListItems.Views
{
    /// <summary>
    /// ThumbnailList.xaml の相互作用ロジック
    /// </summary>
    public partial class ThumbnailList : UserControl
    {
        public ThumbnailList()
        {
            InitializeComponent();
            
            //scroller.ScrollChanged += (object sender, ScrollChangedEventArgs e) =>
            //{
            //    var rect = new Rect(
            //        scroller.HorizontalOffset,
            //        scroller.VerticalOffset,
            //        scroller.ViewportWidth,
            //        scroller.ViewportHeight);
            //    canvas.SetViewport(rect);
            //};
        }

    }
}
