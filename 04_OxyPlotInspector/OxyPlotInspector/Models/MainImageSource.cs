using Prism.Mvvm;
using System.Windows.Media.Imaging;
using ThosoImage.Wpf.Imaging;

namespace OxyPlotInspector.Models
{
    class MainImageSource : BindableBase
    {
        // 画像は固定
        public static readonly string ImageSourcePath = @"C:/data/Image1.jpg";
        private static readonly int ImageViewWidth = 320;
        private static readonly int ImageViewHeight = 240;

        private BitmapImage _ImageSource;
        public BitmapImage ImageSource
        {
            get => _ImageSource;
            private set => SetProperty(ref _ImageSource, value);
        }

        public MainImageSource()
        {
            ImageSource = ImageSourcePath.ToBitmapImage(ImageViewWidth, ImageViewHeight);
        }

    }
}
