using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace ThosoImage.Wpf.Drawing
{
    public static class BitmapImageReaderFromBitmapExtension
    {
        /// <summary>
        /// BitmapをBitmapImageに変換
        /// </summary>
        /// <param name="bitmap">Bitmap画像</param>
        /// <returns>BitmapImage画像</returns>
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            if (bitmap is null) throw new ArgumentNullException();

            var image = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Bmp);

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
            }
            return image;
        }

    }
}
