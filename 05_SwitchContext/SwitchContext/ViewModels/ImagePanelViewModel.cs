using Prism.Mvvm;
using System.Windows.Media.Imaging;
using ThosoImage.Wpf.Imaging;

namespace SwitchContext.ViewModels
{
    class ImagePanelViewModel : BindableBase
    {
        private static readonly string ImagePath1 = @"C:/data/image31.jpg";
        private static readonly string ImagePath2 = @"C:/data/image32.jpg";

        private int _Index;
        public int Index
        {
            get => _Index;
            set
            {
                if (SetProperty(ref _Index, value)) ImageSource = ReadImage(value);
            }
        }

        private BitmapImage _ImageSource;
        public BitmapImage ImageSource
        {
            get => _ImageSource;
            private set => SetProperty(ref _ImageSource, value);
        }

        public ImagePanelViewModel()
        {
            ImageSource = ReadImage(Index);
        }

        private BitmapImage ReadImage(int pattern)
        {
            switch (pattern)
            {
                case 0:
                    return ImagePath1.ToBitmapImage();
                case 1:
                    return ImagePath2.ToBitmapImage();
            }
            return null;
        }

    }
}
