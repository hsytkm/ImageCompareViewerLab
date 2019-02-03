using Prism.Mvvm;
using System.IO;
using System.Windows.Media.Imaging;
using ThosoImage.Wpf.Imaging;

namespace SwitchContext.Models
{
    class MainImage : BindableBase
    {
        public string ImageName { get; }

        private BitmapImage _ImageSource;
        public BitmapImage ImageSource
        {
            get => _ImageSource;
            private set => SetProperty(ref _ImageSource, value);
        }

        public MainImage(string path)
        {
            ImageName = Path.GetFileName(path);
            ImageSource = path.ToBitmapImage();
        }

    }
}
