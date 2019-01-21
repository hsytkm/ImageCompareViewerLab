using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ThosoImage.Wpf.Imaging
{
    public static class BitmapSourceGray
    {
        /// <summary>
        /// 引数PATHを1CHにする
        /// </summary>
        /// <param name="image"></param>
        /// <param name="ch">チャンネル指定(B=0, G=1, R=2)</param>
        /// <returns></returns>
        public static BitmapSource ToGrayBitmapSource(this BitmapSource image, int ch)
        {
            if (image == null) throw new ArgumentNullException();
            if (ch < 0 || 2 < ch) throw new ArgumentException($"Channel Error:{ch}");

            int height = image.PixelHeight;
            int width = image.PixelWidth;
            int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;

            int stride = width * bytesPerPixel;
            var srcData = new byte[height * stride];
            image.CopyPixels(srcData, stride, 0);

            var dstData = new byte[height * width];
            for (int i = 0; i < dstData.Length; i++)
            {
                dstData[i] = srcData[i * bytesPerPixel + ch];
            }

            var bs = BitmapSource.Create(
                width, height,
                image.DpiX, image.DpiY,
                PixelFormats.Gray8, null, dstData, width);
            bs.Freeze();
            return bs;
        }

    }
}
