using Prism.Mvvm;
using System;
using System.Windows.Media.Imaging;

namespace ZoomThumb.Models
{
    class MyImage : BindableBase
    {
        private BitmapSource _ImageSource;
        public BitmapSource ImageSource
        {
            get => _ImageSource;
            private set => SetProperty(ref _ImageSource, value);
        }

        //public Guid guid = Guid.NewGuid();

        public void LoadImage()
        {
            string ImagePath = @"C:\data\Image1.JPG";
            ImageSource = ImagePath.ToBitmapImage();
        }

    }
}
