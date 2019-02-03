using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using ThosoImage.Extensions;

namespace SwitchContext.Models
{
    class MainImages : BindableBase
    {
        private static readonly string ImagePath1 = @"C:/data/image1.jpg";
        private static readonly string ImagePath2 = @"C:/data/image2.jpg";
        private static readonly string ImagePath3 = @"C:/data/image3.jpg";

        // 画像リスト
        public IList<MainImage> ImageSources = new List<MainImage>()
        {
            new MainImage(ImagePath1),
            new MainImage(ImagePath2),
            new MainImage(ImagePath3),
        };

        public BitmapImage GetImageSource(int index)
        {
            if (ImageSources.Count <= index) return null;
            return ImageSources[index].ImageSource;
        }

        // 画像リストの外回りカウンタ
        private int OuterTracksCounter;

        public void RotateOuterTrack()
        {
            OuterTracksCounter = (OuterTracksCounter + 1) % ImageSources.Count;
        }

        // Model画像リストの順序を定着させる
        public void AdaptImageListTracks(int count)
        {
            if (count <= 1) return;
            var list = ImageSources;
            if (count > list.Count) throw new ArgumentException(nameof(count));

            for (int i = 0; i < OuterTracksCounter; i++)
            {
#if false
                var tail = list.ElementAt(count - 1);
                for (int j =  count - 1; j > 0; j--)
                {
                    list[j] = list[j - 1];
                }
                list[0] = tail;
#else
                var head = list.First();
                for (int j = 0; j < count - 1; j++)
                {
                    list[j] = list[j + 1];
                }
                list[count - 1] = head;
#endif
            }
            OuterTracksCounter = 0;
        }

    }
}
