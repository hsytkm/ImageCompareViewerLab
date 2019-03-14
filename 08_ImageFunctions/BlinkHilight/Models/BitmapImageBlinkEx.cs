using System;
using System.Windows.Media.Imaging;

namespace BlinkHilight.Models
{
    public static class BitmapImageBlinkEx
    {
        /// <summary>
        /// 引数画像の飽和画素を塗り潰した画像を返す
        /// </summary>
        /// <param name="source">塗り潰しなしの通常画像</param>
        /// <returns>塗り潰し画像</returns>
        public static BitmapSource ToHighlighBitmapSource(this BitmapSource source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            int height = source.PixelHeight;
            int width = source.PixelWidth;
            int bytesPerPixel = (source.Format.BitsPerPixel + 7) / 8;
            int stride = width * bytesPerPixel;

            var dstData = new byte[height * stride];
            source.CopyPixels(dstData, stride, 0);

            bool rewrite = false;
            for (int i = 0; i < dstData.Length; i += bytesPerPixel)
            {
                byte b = dstData[i + 0];
                byte g = dstData[i + 1];
                byte r = dstData[i + 2];

                // ◆2色飽和は考慮してない
                if (b == 0xff && g == 0xff && r == 0xff)
                {
                    rewrite = true;
                    dstData[i + 0] = 0;
                    dstData[i + 1] = 0;
                    dstData[i + 2] = 0;
                }
                else if (b == 0xff && g != 0xff && r != 0xff)
                {
                    rewrite = true;
                    //dstData[i + 0] = 0xff;
                    dstData[i + 1] = 0;
                    dstData[i + 2] = 0;
                }
                else if (b != 0xff && g == 0xff && r != 0xff)
                {
                    rewrite = true;
                    dstData[i + 0] = 0;
                    //dstData[i + 1] = 0xff;
                    dstData[i + 2] = 0;
                }
                else if (b != 0xff && g != 0xff && r == 0xff)
                {
                    rewrite = true;
                    dstData[i + 0] = 0;
                    dstData[i + 1] = 0;
                    //dstData[i + 2] = 0xff;
                }
            }

            if (!rewrite) return source;

            var bs = BitmapSource.Create(width, height,
                source.DpiX, source.DpiY, source.Format,
                null, dstData, stride);

            bs.Freeze();
            return bs;
        }

    }
}
