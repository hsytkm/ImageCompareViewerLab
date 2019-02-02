using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ThosoImage.Wpf.Imaging
{
    public static class BitmapSourceThumbnailExtension
    {
        private const double DefaultThumbnailWidth = 80.0;

        /// <summary>
        /// 引数PATHをサムネイルとして読み出す
        /// </summary>
        /// <param name="imagePath">ファイルパス</param>
        /// <param name="width">サムネイルの画像幅</param>
        /// <returns></returns>
        public static BitmapSource LoadThumbnail(this string imagePath, double width = DefaultThumbnailWidth)
        {
            if (imagePath is null) throw new ArgumentNullException();
            if (!File.Exists(imagePath)) throw new FileNotFoundException();

            BitmapSource bmp = null;
            using (var stream = File.Open(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var img = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.None);
                if (img.Thumbnail != null)
                {
                    bmp = img.Thumbnail;    // サムネイル読み出し高速化
                }
                else
                {
                    var longSide = Math.Max(img.PixelWidth, img.PixelHeight);
                    var scale = width / longSide;
                    var thumbnail = new TransformedBitmap(img, new ScaleTransform(scale, scale));
                    var cache = new CachedBitmap(thumbnail, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    cache.Freeze();
                    bmp = (BitmapSource)cache;  // アップキャストは常に合法
                }
            }
            return bmp;
        }

    }
}
