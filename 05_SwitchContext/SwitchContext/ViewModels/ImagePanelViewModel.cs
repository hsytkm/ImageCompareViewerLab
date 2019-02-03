using Prism.Mvvm;
using SwitchContext.Models;
using System.Windows.Media.Imaging;
using ThosoImage.Wpf.Imaging;

namespace SwitchContext.ViewModels
{
    class ImagePanelViewModel : BindableBase
    {
        private readonly MainImages MainImages = ModelContext.Instance.MainImages;

        private int _Index;
        public int Index
        {
            get => _Index;
            set
            {
                if (SetProperty(ref _Index, value))
                    ImageSource = MainImages.GetImageSource(Index);
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
            ImageSource = MainImages.GetImageSource(Index);
        }

    }
}
