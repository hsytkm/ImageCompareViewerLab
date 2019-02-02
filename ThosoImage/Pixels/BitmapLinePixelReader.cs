using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ThosoImage.Pixels
{
    public class BitmapLinePixelReader
    {
        // Bgr画素値の二次元配列(width*height)
        private readonly (byte R, byte G, byte B)[,] SourceRgbPixels;

        public BitmapLinePixelReader(string imagePath)
        {
            SourceRgbPixels = ReadRgbPixels(imagePath);
        }

        /// <summary>
        /// 画像ファイルから全画素値を読み出す
        /// </summary>
        /// <param name="imagePath">元画像</param>
        /// <returns>Bgr画素値の二次元配列</returns>
        private (byte R, byte G, byte B)[,] ReadRgbPixels(string imagePath)
        {
            using (var bitmap = new Bitmap(imagePath))
            {
                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                var bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, bitmap.PixelFormat);
                unsafe
                {
                    try
                    {
                        var rgbs = new (byte R, byte G, byte B)[bitmap.Width, bitmap.Height];

                        var stride = bitmapData.Stride;
                        var ptrSt = (byte*)bitmapData.Scan0;
                        var ptrEd = ptrSt + bitmap.Height * stride;
                        var xEd = bitmap.Width * bytesPerPixel;

                        int y = 0;
                        for (byte* pixels = ptrSt; pixels < ptrEd; pixels += stride, y++)
                        {
                            for (int px = 0, x = 0; px < xEd; px += bytesPerPixel, x++)
                            {
                                rgbs[x, y] = (pixels[px + 2], pixels[px + 1], pixels[px]);
                            }
                        }
                        return rgbs;
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                }
            }
        }

        /// <summary>
        /// 読み込み済み画素値から2点を結ぶ線上の画素値を返します
        /// </summary>
        /// <param name="point1x">開始点Xの割合(0~1)</param>
        /// <param name="point1y">開始点Yの割合(0~1)</param>
        /// <param name="point2x">終了点Xの割合(0~1)</param>
        /// <param name="point2y">終了点Yの割合(0~1)</param>
        /// <returns>RGBの画素値配列(8bit)</returns>
        public (byte R, byte G, byte B)[] GetRgbLineLevelsRatio(double point1XRatio, double point1YRatio, double point2XRatio, double point2YRatio)
        {
            try
            {
                var sourceRgbPixels = SourceRgbPixels;
                int imageWidth = sourceRgbPixels.GetLength(0);
                int imageHeight = sourceRgbPixels.GetLength(1);
                int point1x = (int)(point1XRatio * imageWidth);
                int point1y = (int)(point1YRatio * imageHeight);
                int point2x = (int)(point2XRatio * imageWidth);
                int point2y = (int)(point2YRatio * imageHeight);

                double distance = Math.Sqrt((point1x - point2x) * (point1x - point2x)
                    + (point1y - point2y) * (point1y - point2y));

                var rgbs = new (byte R, byte G, byte B)[(int)distance];
                for (int i = 0; i < rgbs.Length; i++)
                {
                    int x = (int)Math.Floor(point1x + ((point2x - point1x) * i / distance));
                    int y = (int)Math.Floor(point1y + ((point2y - point1y) * i / distance));
                    rgbs[i] = sourceRgbPixels[x, y];
                }
                return rgbs;
            }
            catch (Exception) { throw; }
        }

        #region static methods

        /// <summary>
        /// 引数画像の2点を結ぶ線上の画素値を返します(1度しか読み込まない場面用)
        /// </summary>
        /// <param name="imagePath">解析対象の画像PATH</param>
        /// <param name="point1x">開始点Xの割合(0~1)</param>
        /// <param name="point1y">開始点Yの割合(0~1)</param>
        /// <param name="point2x">終了点Xの割合(0~1)</param>
        /// <param name="point2y">終了点Yの割合(0~1)</param>
        /// <returns>RGBの画素値配列(8bit)</returns>
        public static (byte R, byte G, byte B)[] GetRgbLineLevels(string imagePath,
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

                var rgbs = new (byte R, byte G, byte B)[(int)distance];

                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                var bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, bitmap.PixelFormat);

                try
                {
                    unsafe
                    {
                        for (int i = 0; i < rgbs.Length; i++)
                        {
                            int xIndex = (int)Math.Floor(point1x + ((point2x - point1x) * i / distance));
                            int yIndex = (int)Math.Floor(point1y + ((point2y - point1y) * i / distance));
                            var pixels = (byte*)bitmapData.Scan0
                                + (yIndex * bitmapData.Stride)
                                + (xIndex * bytesPerPixel);
                            rgbs[i] = (pixels[2], pixels[1], pixels[0]);
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

        #endregion

    }
}
