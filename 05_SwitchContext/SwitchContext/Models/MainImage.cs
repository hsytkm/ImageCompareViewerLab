using Prism.Mvvm;
using System.IO;
using System.Windows.Media.Imaging;
using ThosoImage.Wpf.Imaging;

namespace SwitchContext.Models
{
    class MainImage : BindableBase
    {
        public string ImageName { get; }
        public BitmapImage ImageSource { get; }

        public MainImage(string path)
        {
            ImageName = Path.GetFileName(path);
            ImageSource = path.ToBitmapImage();
        }

    }
}
