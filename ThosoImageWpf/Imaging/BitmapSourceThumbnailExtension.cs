using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ThosoImage.Wpf.Imaging
{
    public static class BitmapSourceThumbnailExtension
    {
        private const int DefaultThumbnailWidth = 120;

        /// <summary>
        /// 引数PATHをサムネイルとして読み出す
        /// </summary>
        /// <param name="imagePath">ファイルパス</param>
        /// <param name="width">サムネイルの画像幅</param>
        /// <returns></returns>
        public static BitmapSource LoadThumbnail(this string imagePath, int width = DefaultThumbnailWidth)
        {
            if (imagePath is null) throw new ArgumentNullException(nameof(imagePath));
            if (!File.Exists(imagePath)) throw new FileNotFoundException(imagePath);

            using (var stream = File.Open(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapSource bitmapSource = null;

                var img = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.None);
                if (img.Thumbnail != null)
                {
                    bitmapSource = img.Thumbnail;    // サムネイル読み出し高速化
                }
                else
                {
                    bitmapSource = img as BitmapSource;
                }

                var longSide = Math.Max(bitmapSource.PixelWidth, bitmapSource.PixelHeight);
                var scale = width / (double)longSide;
                var thumbnail = new TransformedBitmap(bitmapSource, new ScaleTransform(scale, scale));
                var cachedBitmap = new CachedBitmap(thumbnail, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                cachedBitmap.Freeze();

                return (BitmapSource)cachedBitmap;  // アップキャストは常に合法
            }
        }

    }
}
