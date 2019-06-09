using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ZoomThumb.Views.Common;

namespace ZoomThumb.Views.Controls
{
    /// <summary>
    /// ImageOverlapSamplingFrame.xaml の相互作用ロジック
    /// </summary>
    public partial class ImageOverlapSamplingFrame : UserControl
    {
        private static readonly Type SelfType = typeof(ImageOverlapSamplingFrame);

        #region FrameRectRatioProperty(TwoWay)

        // サンプリング枠の表示位置の割合
        private static readonly DependencyProperty FrameRectRatioProperty =
            DependencyProperty.Register(
                nameof(FrameRectRatio),
                typeof(Rect),
                SelfType,
                new FrameworkPropertyMetadata(
                    default(Rect),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) =>
                    {
                        if (e.NewValue is Rect rect)
                        {
                        }
                    }));

        public Rect FrameRectRatio
        {
            get => (Rect)GetValue(FrameRectRatioProperty);
            set => SetValue(FrameRectRatioProperty, value);
        }

        #endregion

        #region FrameBorderBrushProperty(OneWay)

        // 枠色
        private static readonly DependencyProperty FrameBorderBrushProperty =
            DependencyProperty.Register(
                nameof(FrameBorderBrush),
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.None));

        public Brush FrameBorderBrush
        {
            get => (Brush)GetValue(FrameBorderBrushProperty);
            set => SetValue(FrameBorderBrushProperty, value);
        }

        #endregion

        #region GroundPanelSizeProperty(OneWay)

        // 下地パネルのサイズ
        private static readonly DependencyProperty GroundPanelSizeProperty =
            DependencyProperty.Register(
                nameof(GroundPanelSize),
                typeof(Size),
                SelfType,
                new FrameworkPropertyMetadata(
                    default(Size),
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) =>
                    {
                        if (ViewHelper.GetChildControl<Canvas>(d) is Canvas canvas
                        && e.NewValue is Size newSize)
                        {
                            canvas.Width = newSize.Width;
                            canvas.Height = newSize.Height;
                        }
                    }));

        public Size GroundPanelSize
        {
            get => (Size)GetValue(GroundPanelSizeProperty);
            set => SetValue(GroundPanelSizeProperty, value);
        }

        #endregion

        public ImageOverlapSamplingFrame()
        {
            InitializeComponent();
        }

    }
}
