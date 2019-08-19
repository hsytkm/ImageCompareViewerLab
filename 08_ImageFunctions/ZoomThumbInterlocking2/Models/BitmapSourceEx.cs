using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace ZoomThumb.Models
{
    public static class BitmapSourceEx
    {
        /// <summary>
        /// 引数PATHを画像として読み出す
        /// </summary>
        /// <param name="imagePath">ファイルパス</param>
        /// <returns></returns>
        public static BitmapImage ToBitmapImage(this string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException(imagePath);

            var bi = new BitmapImage();
            using (var fs = File.Open(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = fs;
                bi.EndInit();
            }
            bi.Freeze();

            if (bi.Width == 1 && bi.Height == 1) throw new OutOfMemoryException();
            return bi;
        }
    }
}
