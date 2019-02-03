using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using ThosoImage.Extensions;

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
        private int InnerTracksCounter;

        public void RotateInnerTrack()
        {
            InnerTracksCounter = (InnerTracksCounter + 1) % ImageSources.Count;
        }

        // Model画像リストの順序を定着させる
        public void AdaptImageListTracks()
        {
            ImageSources.RotateAscendingOrder(InnerTracksCounter);
            InnerTracksCounter = 0;
        }

    }
}
