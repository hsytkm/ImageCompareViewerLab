using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ThosoImage.Wpf.Imaging
{
    public static class BitmapSourceRotate
    {
        /// <summary>
        /// 回転した画像を取得する
        /// </summary>
        /// <param name="source">基準画像</param>
        /// <param name="angle">回転角</param>
        /// <returns>回転した画像</returns>
        public static BitmapSource Rotation(this BitmapSource source, int angle)
        {
            if (source is null) throw new ArgumentNullException();

            angle %= 360;
            if (angle == 0) return source;

            var bitmap = new TransformedBitmap();
            bitmap.BeginInit();
            bitmap.Source = source;
            bitmap.Transform = new RotateTransform(angle);
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

    }
}
