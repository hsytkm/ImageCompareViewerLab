using Prism.Mvvm;
using System.IO;
using System.Windows.Media.Imaging;
using ThosoImage.Wpf.Imaging;

namespace VirtualizationListItems.Models
{
    class ImageSource : BindableBase
    {
        private const int ThumbnailWidth = 80;

        private string _FilePath;
        public string FilePath
        {
            get => _FilePath;
            private set => SetProperty(ref _FilePath, value);
        }

        public string Filename { get => Path.GetFileName(_FilePath); }

        private BitmapSource _Thumbnail;
        public BitmapSource Thumbnail
        {
            get => _Thumbnail;
            private set => SetProperty(ref _Thumbnail, value);
        }

        public bool IsThumbnailEmpty { get => Thumbnail == null; }

        public ImageSource(string path)
        {
            FilePath = path;
        }

        public void LoadThmbnail()
        {
            if (Thumbnail == null)
                Thumbnail = FilePath.LoadThumbnail(ThumbnailWidth);
        }

        public void UnloadThmbnail()
        {
            Thumbnail = null;
        }

    }
}
