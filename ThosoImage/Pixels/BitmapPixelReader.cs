using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ThosoImage.Pixels
{
    public static class BitmapPixelReader
    {

        /// <summary>
        /// 引数画像の2点を結ぶ線上の画素値を返します
        /// </summary>
        /// <param name="imagePath">解析対象の画像PATH</param>
        /// <param name="point1x">開始点Xの割合(0~1)</param>
        /// <param name="point1y">開始点Yの割合(0~1)</param>
        /// <param name="point2x">終了点Xの割合(0~1)</param>
        /// <param name="point2y">終了点Yの割合(0~1)</param>
        /// <returns>RGBの画素値配列(8bit)</returns>
        public static (byte R, byte G, byte B)[] GetRgbLineLevels(this string imagePath,
            double point1XRatio, double point1YRatio, double point2XRatio, double point2YRatio)
        {
            if (imagePath is null) throw new ArgumentNullException();
            if (!File.Exists(imagePath)) throw new FileNotFoundException();

            try
            {
                using (var bitmap = new Bitmap(imagePath))
                {
                    int point1x = (int)(point1XRatio * bitmap.Width);
                    int point1y = (int)(point1YRatio * bitmap.Height);
                    int point2x = (int)(point2XRatio * bitmap.Width);
                    int point2y = (int)(point2YRatio * bitmap.Height);
                    return GetRgbLineLevels(bitmap, point1x, point1y, point2x, point2y);
                }
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 引数画像の2点を結ぶ線上の画素値を返します
        /// </summary>
        /// <param name="bitmap">解析対象の画像</param>
        /// <param name="point1x">開始点Xの実座標</param>
        /// <param name="point1y">開始点Yの実座標</param>
        /// <param name="point2x">終了点Xの実座標</param>
        /// <param name="point2y">終了点Yの実座標</param>
        /// <returns>RGBの画素値配列(8bit)</returns>
        private static (byte R, byte G, byte B)[] GetRgbLineLevels(Bitmap bitmap,
            int point1x, int point1y, int point2x, int point2y)
        {
            try
            {
                double distance = Math.Sqrt((point1x - point2x) * (point1x - point2x)
                    + (point1y - point2y) * (point1y - point2y));
                int distFloor = (int)Math.Floor(distance);

                var rgbs = new (byte R, byte G, byte B)[distFloor];

                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                var bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, bitmap.PixelFormat);

                try
                {
                    for (int pt = 0; pt < distFloor; pt++)
                    {
                        int xIndex = (int)Math.Floor(point1x + ((point2x - point1x) * pt / distance));
                        int yIndex = (int)Math.Floor(point1y + ((point2y - point1y) * pt / distance));

                        unsafe
                        {
                            var pixels = (byte*)bitmapData.Scan0
                                + (yIndex * bitmapData.Stride)
                                + (xIndex * bytesPerPixel);
                            rgbs[pt] = (pixels[2], pixels[1], pixels[0]);
                        }
                    }
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                return rgbs;
            }
            catch (Exception) { throw; }
        }

    }
}
