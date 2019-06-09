using Reactive.Bindings;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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

        public ImageOverlapSamplingFrame()
        {
            InitializeComponent();

            this.Loaded += (_, __) =>
            {
                var scrollViewer = ViewHelper.GetChildControl<ScrollViewer>(this.Parent);
                if (scrollViewer != null)
                {
                    var scrollContentPresenter = ViewHelper.GetChildControl<ScrollContentPresenter>(scrollViewer);
                    if (scrollContentPresenter != null)
                    {
                        //ScrollContentActualSize.Value = ViewHelper.GetControlActualSize(scrollContentPresenter);
                        GroundCanvas.Width = scrollContentPresenter.ActualWidth;
                        GroundCanvas.Height = scrollContentPresenter.ActualHeight;
                        scrollContentPresenter.SizeChanged += (sender, e) =>
                        {
                            GroundCanvas.Width = e.NewSize.Width; //=ActualSize
                            GroundCanvas.Height = e.NewSize.Height;
                        };
                    }
                }

                var mainImage = ViewHelper.GetChildControl<Image>(scrollViewer);
                if (mainImage != null)
                {
                    mainImage.SizeChanged += (sender, e) =>
                    {
                        //ImageViewActualSize.Value = e.NewSize; //=ActualSize

                        //GroundCanvas.Width = e.NewSize.Width; //=ActualSize
                        //GroundCanvas.Height = e.NewSize.Height;
                    };
                }
            };



        }


    }
}
