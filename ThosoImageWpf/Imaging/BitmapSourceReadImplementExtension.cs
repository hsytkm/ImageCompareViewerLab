using ThosoImage.ColorSpace;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ThosoImage.Wpf.Imaging
{
    static class BitmapSourceReadImplementExtension
    {
        // Rectの範囲制限
        private static Int32Rect ClipRect(ref Int32Rect rectInput, int width, int height)
        {
            int clip(int val, int min, int max)
            {
                if (val <= min) return min;
                if (val >= max) return max;
                return val;
            }
            var rectX = clip(rectInput.X, 0, width);
            var rectY = clip(rectInput.Y, 0, height);
            return new Int32Rect(rectX, rectY,
                clip(rectInput.Width, 1, width - rectX),
                clip(rectInput.Height, 1, height - rectY));
        }

        /// <summary>
        /// 指定1画素のGamutを返す
        /// </summary>
        /// <param name="bitmap">対象画像</param>
        /// <param name="pointRate">対象画素(割合)</param>
        /// <returns>画素値(BGR)</returns>
        public static Gamut ReadPixel(this BitmapSource bitmap, Point pointRate)
        {
            int rectX = (int)Math.Round(pointRate.X * bitmap.PixelWidth);
            int rectY = (int)Math.Round(pointRate.Y * bitmap.PixelHeight);
            return bitmap.GetPixelAverage(new Int32Rect(rectX, rectY, 1, 1));
        }

        /// <summary>
        /// 指定矩形のGamutを返す
        /// </summary>
        /// <param name="bitmap">対象画像</param>
        /// <param name="rectRate">対象領域(割合)</param>
        /// <returns>画素値(BGR)</returns>
        public static Gamut ReadPixelsAverage(this BitmapSource bitmap, Rect rectRate)
        {
            int rectX = (int)Math.Round(rectRate.X * bitmap.PixelWidth);
            int rectY = (int)Math.Round(rectRate.Y * bitmap.PixelHeight);
            int rectWidth = (int)Math.Round(rectRate.Width * bitmap.PixelWidth);
            int rectHeight = (int)Math.Round(rectRate.Height * bitmap.PixelHeight);
            return bitmap.GetPixelAverage(new Int32Rect(rectX, rectY, rectWidth, rectHeight));
        }

        // 指定領域の平均輝度の読み込み
        private static Gamut GetPixelAverage(this BitmapSource bitmap, Int32Rect rectInput)
        {
            if (bitmap is null) throw new ArgumentNullException(nameof(bitmap));
            if (rectInput.Width * rectInput.Height == 0) throw new ArgumentException("RectArea");

            int pixelsByte = (bitmap.Format.BitsPerPixel + 7) / 8; // bit→Byte変換
            int imageWidth = bitmap.PixelWidth;
            int imageHeight = bitmap.PixelHeight;
            var rect = ClipRect(ref rectInput, imageWidth, imageHeight);
            int rectArea = rect.Width * rect.Height;

            var cb = new CroppedBitmap(bitmap, rect);
            var pixels = new byte[rectArea * pixelsByte];
            try
            {
                cb.CopyPixels(pixels, rect.Width * pixelsByte, 0);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Trace.WriteLine(ex.Message);    // 謎たまに起きる
            }

            // 1画素(カーソル用)の計算
            if (rectArea == 1)
            {
                if (pixelsByte < 3) return new Gamut(pixels[0]);
                return new Gamut(r: pixels[2], g: pixels[1], b: pixels[0]);
            }
            else
            {
                if (pixelsByte <= 1)
                    return GetGamut1ch(pixels, rect.Width * rect.Height, pixelsByte);
                return GetGamut3ch(pixels, rect.Width * rect.Height, pixelsByte);
            }
        }

        #region 1ch

        private static Gamut GetGamut1ch(byte[] pixels, int count, int pixelsByte)
        {
            var ave = ReadAverage1ch(pixels, count, pixelsByte);
            var rms = ReadRms1ch(pixels, count, pixelsByte, ave);
            return new Gamut(ave, rms);
        }

        // 1ch画素の平均値を計算
        private static double ReadAverage1ch(byte[] pixels, int count, int pixelsByte)
        {
            ulong sum = 0;
            for (var i = 0; i < pixels.Length; i += pixelsByte)
            {
                sum += pixels[i];
            }
            return sum / (double)count;
        }

        // ROIの二乗平均平方根
        private static double ReadRms1ch(byte[] pixels, int count, int pixelsByte, double ave)
        {
            double sum = 0D;
            for (var i = 0; i < pixels.Length; i += pixelsByte)
            {
                var d = pixels[i] - ave;
                sum += d * d;
            }
            return sum / count;
        }

        #endregion

        #region 3ch

        private static Gamut GetGamut3ch(byte[] pixels, int count, int pixelsByte)
        {
            var ave = ReadRgbAverage3ch(pixels, count, pixelsByte);
            var rms = ReadRgbyRms3ch(pixels, count, pixelsByte, ave);
            return new Gamut(ave, rms);
        }

        // 3ch画素の平均値を計算
        private static (double R, double G, double B)
            ReadRgbAverage3ch(byte[] pixels, int count, int pixelsByte)
        {
            ulong sumB = 0, sumG = 0, sumR = 0;
            for (var i = 0; i < pixels.Length; i += pixelsByte)
            {
                sumB += pixels[i];
                sumG += pixels[i + 1];
                sumR += pixels[i + 2];
            }
            var aveR = sumR / (double)count;
            var aveG = sumG / (double)count;
            var aveB = sumB / (double)count;
            return (aveR, aveG, aveB);
        }

        // ROIの二乗平均平方根
        private static (double R, double G, double B, double Y)
            ReadRgbyRms3ch(byte[] pixels, int count, int pixelsByte, (double R, double G, double B) ave)
        {
            var aveY = Gamut.CalcY(r: ave.R, g: ave.G, b: ave.B);
            double sumB = 0D, sumG = 0D, sumR = 0D, sumY = 0D;

            for (var i = 0; i < pixels.Length; i += pixelsByte)
            {
                var b = pixels[i];
                var g = pixels[i + 1];
                var r = pixels[i + 2];
                var y = Gamut.CalcY(r: r, g: g, b: b);

                // Math.Powで2乗するよりベタの方が速い
                sumB += (b - ave.B) * (b - ave.B);
                sumG += (g - ave.G) * (g - ave.G);
                sumR += (r - ave.R) * (r - ave.R);
                sumY += (y - aveY) * (y - aveY);
            }

            var rmsR = Math.Sqrt(sumR / count);
            var rmsG = Math.Sqrt(sumG / count);
            var rmsB = Math.Sqrt(sumB / count);
            var rmsY = Math.Sqrt(sumY / count);
            return (rmsR, rmsG, rmsB, rmsY);
        }

        #endregion

    }
}
