using Prism.Mvvm;
using System.IO;
using System.Windows.Media.Imaging;

namespace ZoomThumb.Models
{
    class MainImage : BindableBase
    {
        public string ImagePath { get; }
        public string ImageName { get; }

        private BitmapSource _ImageSource;
        public BitmapSource ImageSource
        {
            get => _ImageSource;
            private set => SetProperty(ref _ImageSource, value);
        }

        public MainImage(string path)
        {
            ImagePath = path;
            ImageName = Path.GetFileName(path);
        }

        public void LoadImage()
        {
            if (ImageSource is null)
                ImageSource = ImagePath.ToBitmapImage();
        }

    }
}
