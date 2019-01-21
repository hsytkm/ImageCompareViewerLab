using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ThosoImage.Wpf.Drawing
{
    public static class BitmapImageReaderFromBitmap
    {
#if false   // Drawingを参照するのイヤなので無効化

        /// <summary>
        /// BitmapをBitmapImageに変換
        /// </summary>
        /// <param name="bitmap">Bitmap画像</param>
        /// <returns>BitmapImage画像</returns>
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException();

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
#endif

    }
}
