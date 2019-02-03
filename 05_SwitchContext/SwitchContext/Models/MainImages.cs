using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace SwitchContext.Models
{
    class MainImages : BindableBase
    {
        private static readonly string ImagePath1 = @"C:/data/image1.jpg";
        private static readonly string ImagePath2 = @"C:/data/image2.jpg";

        // 画像リスト
        public IList<MainImage> ImageSources = new List<MainImage>()
        {
            new MainImage(ImagePath1),
            new MainImage(ImagePath2),
        };

        public BitmapImage GetImageSource(int index)
        {
            if (ImageSources.Count <= index) return null;
            return ImageSources[index].ImageSource;
        }

        // 画像リストの内回りカウンタ
        public int InnerTracksCounter { get; set; }

        // Model画像リストの順序を定着させる
        public void AdaptImageListTracks()
        {

        }

    }
}
