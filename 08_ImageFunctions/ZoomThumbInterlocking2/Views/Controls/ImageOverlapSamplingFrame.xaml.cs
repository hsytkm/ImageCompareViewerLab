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

        //private readonly ReactivePropertySlim<Size> ImageViewActualSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.DistinctUntilChanged);

        #region FrameRectRateProperty(TwoWay)

        // サンプリング枠の表示位置の割合
        private static readonly DependencyProperty FrameRectRateProperty =
            DependencyProperty.Register(
                nameof(FrameRectRate),
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

        public Rect FrameRectRate
        {
            get => (Rect)GetValue(FrameRectRateProperty);
            set => SetValue(FrameRectRateProperty, value);
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
                }

                var mainImage = ViewHelper.GetChildControl<Image>(scrollViewer);
                if (mainImage != null)
                {
                    mainImage.SizeChanged += (sender, e) =>
                    {
                        //ImageViewActualSize.Value = e.NewSize; //=ActualSize

                        GroundCanvas.Width = e.NewSize.Width; //=ActualSize
                        GroundCanvas.Height = e.NewSize.Height;
                    };
                }
            };



        }


    }
}
