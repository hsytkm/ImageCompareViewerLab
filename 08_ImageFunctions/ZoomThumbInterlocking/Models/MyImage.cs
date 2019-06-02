using Prism.Mvvm;
using System;
using System.Windows.Media.Imaging;

namespace ZoomThumb.Models
{
    class MyImage : BindableBase
    {
        private string _ImagePath;
        private string ImagePath
        {
            set
            {
                if (SetProperty(ref _ImagePath, value))
                    ImageSource = _ImagePath.ToBitmapImage();
            }
        }

        private BitmapSource _ImageSource;
        public BitmapSource ImageSource
        {
            get => _ImageSource;
            private set => SetProperty(ref _ImageSource, value);
        }

        //public Guid guid = Guid.NewGuid();

        public void LoadImage(string imagePath)
        {
            ImagePath = imagePath;
        }

    }
}
