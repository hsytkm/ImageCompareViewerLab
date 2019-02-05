using Prism.Mvvm;
using SwitchContext.Models;
using System.Windows.Media.Imaging;

namespace SwitchContext.ViewModels
{
    class ImagePanelViewModel : BindableBase
    {
        private readonly MainImages MainImages = ModelContext.Instance.MainImages;
        
        private BitmapImage _ImageSource;
        public BitmapImage ImageSource
        {
            get => _ImageSource;
            private set => SetProperty(ref _ImageSource, value);
        }

        public ImagePanelViewModel() { }

        public void UpdateImageSource(int index)
        {
            ImageSource = MainImages.GetImageSource(index);
        }

    }
}
