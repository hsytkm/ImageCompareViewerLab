using Prism.Mvvm;
using System;

namespace ZoomThumb.Models
{
    class MainImages : BindableBase
    {
        private static readonly string ImagePath1 = @"C:/data/image1.jpg";
        private static readonly string ImagePath2 = @"C:/data/image2.jpg";
        private static readonly string ImagePath3 = @"C:/data/image3.jpg";

        // 画像リスト
        public readonly MainImage[] ImageSources = new[]
        {
            new MainImage(ImagePath1),
            new MainImage(ImagePath2),
            new MainImage(ImagePath3),
        };

        private int ImageReferenceCounter = 0;

        // 参照されていないインデックスを返す（テキトー実装）
        public int GetImageIndex()
        {
            int index = ImageReferenceCounter;
            int max = ImageSources.Length - 1;

            ImageReferenceCounter++;

            return (index <= max) ? index : max;
        }

        public void LoadImage(int index)
        {
            if (ImageSources.Length <= index) return;
            ImageSources[index].LoadImage();
        }

    }
}
