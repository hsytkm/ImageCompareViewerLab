using Prism.Mvvm;
using SwitchContext.Models;
using System.Windows.Media.Imaging;

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
                if (SetProperty(ref _Index, value)) UpdateImageSource(Index);
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
            UpdateImageSource(Index);
        }

        public void UpdateImageSource(int index)
        {
            Index = index;
            ImageSource = MainImages.GetImageSource(Index);
        }

    }
}
