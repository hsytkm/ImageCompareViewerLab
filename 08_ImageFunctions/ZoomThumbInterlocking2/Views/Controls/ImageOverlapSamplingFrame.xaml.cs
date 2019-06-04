using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ZoomThumb.Views.Common;

namespace ZoomThumb.Views.Controls
{
    /// <summary>
    /// ImageOverlapSamplingFrame.xaml の相互作用ロジック
    /// </summary>
    public partial class ImageOverlapSamplingFrame : UserControl
    {
        #region ImageViewActualSizeProperty(OneWay)

        // 画像コントロールのサイズ
        private static readonly string ImageViewActualSize = nameof(ImageViewActualSize);

        private static readonly DependencyProperty ImageViewActualSizeProperty =
            DependencyProperty.Register(
                nameof(ImageViewActualSize),
                typeof(Size),
                typeof(ImageOverlapSamplingFrame),
                new FrameworkPropertyMetadata(
                    default(Size),
                    FrameworkPropertyMetadataOptions.None));

        public static Size GetImageViewActualSize(DependencyObject depObj) =>
            (Size)depObj.GetValue(ImageViewActualSizeProperty);

        public static void SetImageViewActualSize(DependencyObject depObj, Size value) =>
            depObj.SetValue(ImageViewActualSizeProperty, value);

        #endregion

        public ImageOverlapSamplingFrame()
        {
            InitializeComponent();
        }


    }
}
