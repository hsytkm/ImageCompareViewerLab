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
        private static readonly DependencyProperty ImageViewActualSizeProperty =
            DependencyProperty.Register(
                nameof(ImageViewActualSize),
                typeof(Size),
                typeof(ImageOverlapSamplingFrame),
                new FrameworkPropertyMetadata(
                    default(Size),
                    FrameworkPropertyMetadataOptions.None));

        public Size ImageViewActualSize
        {
            get => (Size)GetValue(ImageViewActualSizeProperty);
            set => SetValue(ImageViewActualSizeProperty, value);
        }

        #endregion

        public ImageOverlapSamplingFrame()
        {
            InitializeComponent();
        }


    }
}
