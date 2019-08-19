using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace ThosoImage.ColorSpace
{
    internal static class GamutReaderImplementExtension
    {
        // Rectangleの範囲制限
        private static Rectangle ClipRectangle(Rectangle rectInput, int width, int height)
        {
            int clip(int val, int min, int max)
            {
                if (val <= min) return min;
                if (val >= max) return max;
                return val;
            }
            var rectX = clip(rectInput.X, 0, width);
            var rectY = clip(rectInput.Y, 0, height);
            return new Rectangle(rectX, rectY,
                clip(rectInput.Width, 1, width - rectX),
                clip(rectInput.Height, 1, height - rectY));
        }

        // 単一エリアの計算
        internal static Gamut ReadGamutRgb(this Bitmap bitmap, Rectangle rectInput)
        {
            if (bitmap is null) throw new ArgumentNullException(nameof(bitmap));

            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            try
            {
                return GetGamut(bitmapData, bytesPerPixel,
                    ClipRectangle(rectInput, bitmap.Width, bitmap.Height));
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        // 複数エリアの計算
        internal static IEnumerable<Gamut> ReadGamutRgb(this Bitmap bitmap, IReadOnlyList<Rectangle> rects)
        {
            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            try
            {
                foreach (var rectInput in rects)
                {
                    yield return GetGamut(bitmapData, bytesPerPixel,
                        ClipRectangle(rectInput, bitmap.Width, bitmap.Height));
                }
            }
            finally {
                bitmap.UnlockBits(bitmapData);
            }
        }

        // Gamutの読み出し
        private static Gamut GetGamut(BitmapData bitmapData, int bytesPerPixel, Rectangle rect)
        {
            var rgbAverage = ReadRgbAverage(bitmapData, bytesPerPixel, ref rect);
            var rgbyRms = ReadRgbyRms(bitmapData, bytesPerPixel, ref rect, rgbAverage);
            return new Gamut(rgbAverage, rgbyRms);
        }

        // 平均値の読み出し ProcessUsingLockbitsAndUnsafe()
        private static (double R, double G, double B)
            ReadRgbAverage(BitmapData bitmapData, int bytesPerPixel, ref Rectangle rect)
        {
            ulong sumB = 0, sumG = 0, sumR = 0;
            unsafe
            {
                var stride = bitmapData.Stride;
                var ptrSt = (byte*)bitmapData.Scan0 + rect.Y * stride;
                var ptrEd = ptrSt + rect.Height * stride;
                var xSt = rect.X * bytesPerPixel;
                var xEd = (rect.X + rect.Width) * bytesPerPixel;
                for (byte* pixels = ptrSt; pixels < ptrEd; pixels += stride)
                {
                    for (int x = xSt; x < xEd; x += bytesPerPixel)
                    {
                        sumB += pixels[x];
                        sumG += pixels[x + 1];
                        sumR += pixels[x + 2];
                    }
                }
            }

            var count = (double)(rect.Width * rect.Height);
            var aveR = sumR / count;
            var aveG = sumG / count;
            var aveB = sumB / count;
            return (aveR, aveG, aveB);
        }

        // ROIの二乗平均平方根
        private static (double R, double G, double B, double Y)
            ReadRgbyRms(BitmapData bitmapData, int bytesPerPixel, ref Rectangle rect, (double R, double G, double B) ave)
        {
            var aveY = Gamut.CalcY(r: ave.R, g: ave.G, b: ave.B);
            double sumB = 0D, sumG = 0D, sumR = 0D, sumY = 0D;

            unsafe
            {
                var stride = bitmapData.Stride;
                var ptrSt = (byte*)bitmapData.Scan0 + rect.Y * stride;
                var ptrEd = ptrSt + rect.Height * stride;
                var xSt = rect.X * bytesPerPixel;
                var xEd = (rect.X + rect.Width) * bytesPerPixel;
                for (byte* pixels = ptrSt; pixels < ptrEd; pixels += stride)
                {
                    for (int x = xSt; x < xEd; x += bytesPerPixel)
                    {
                        var b = pixels[x];
                        var g = pixels[x + 1];
                        var r = pixels[x + 2];
                        var y = Gamut.CalcY(r: r, g: g, b: b);

                        // Math.Powで2乗するよりベタの方が速い
                        sumB += (b - ave.B) * (b - ave.B);
                        sumG += (g - ave.G) * (g - ave.G);
                        sumR += (r - ave.R) * (r - ave.R);
                        sumY += (y - aveY) * (y - aveY);
                    }
                }
            }

            var count = (double)(rect.Width * rect.Height);
            var rmsR = Math.Sqrt(sumR / count);
            var rmsG = Math.Sqrt(sumG / count);
            var rmsB = Math.Sqrt(sumB / count);
            var rmsY = Math.Sqrt(sumY / count);
            return (rmsR, rmsG, rmsB, rmsY);
        }

    }
}
