using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ThosoImage.Wpf.Imaging
{
    public static class BitmapSourceRotate
    {
        /// <summary>
        /// 画像の回転
        /// </summary>
        public static BitmapSource Rotation(this BitmapSource source, int angle)
        {
            if (source == null) throw new ArgumentNullException();

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
