﻿using Prism.Mvvm;
using System.Windows.Media.Imaging;
using ThosoImage.Wpf.Imaging;

namespace SwitchContext.Models
{
    class MainImage : BindableBase
    {
        private BitmapImage _ImageSource;
        public BitmapImage ImageSource
        {
            get => _ImageSource;
            private set => SetProperty(ref _ImageSource, value);
        }

        public MainImage(string path)
        {
            ImageSource = path.ToBitmapImage();
        }

    }
}
