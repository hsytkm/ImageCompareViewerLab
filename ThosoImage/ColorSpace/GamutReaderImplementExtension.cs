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
            int limit(int val, int min, int max)
            {
                if (val <= min) return min;
                if (val >= max) return max;
                return val;
            }
            var rectX = limit(rectInput.X, 0, width);
            var rectY = limit(rectInput.Y, 0, height);
            return new Rectangle(rectX, rectY,
                limit(rectInput.Width, 1, width - rectX),
                limit(rectInput.Height, 1, height - rectY));
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
            finally
            {
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

        // 画素値ポインタ
        private static unsafe ReadOnlySpan<byte> GetPixelsSpan(BitmapData bitmapData) =>
            new ReadOnlySpan<byte>((byte*)bitmapData.Scan0, bitmapData.Height * bitmapData.Stride);

        // 平均値の読み出し ProcessUsingLockbitsAndUnsafe()
        private static (double R, double G, double B)
            ReadRgbAverage(BitmapData bitmapData, int bytesPerPixel, ref Rectangle rect)
        {
            ulong sumB = 0, sumG = 0, sumR = 0;

            var stride = bitmapData.Stride;
            var ySt = rect.Y * stride;
            var yEd = (rect.Y + rect.Height) * stride;
            var xSt = rect.X * bytesPerPixel;
            var xEd = (rect.X + rect.Width) * bytesPerPixel;
            var pixels = GetPixelsSpan(bitmapData);

            for (int y = ySt; y < yEd; y += stride)
            {
                for (int n = y + xSt; n < y + xEd; n += bytesPerPixel)
                {
                    sumB += pixels[n];
                    sumG += pixels[n + 1];
                    sumR += pixels[n + 2];
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

            var stride = bitmapData.Stride;
            var ySt = rect.Y * stride;
            var yEd = (rect.Y + rect.Height) * stride;
            var xSt = rect.X * bytesPerPixel;
            var xEd = (rect.X + rect.Width) * bytesPerPixel;
            var pixels = GetPixelsSpan(bitmapData);

            for (int y = ySt; y < yEd; y += stride)
            {
                for (int n = y + xSt; n < y + xEd; n += bytesPerPixel)
                {
                    var pb = pixels[n];
                    var pg = pixels[n + 1];
                    var pr = pixels[n + 2];
                    var py = Gamut.CalcY(r: pr, g: pg, b: pb);

                    // Math.Powで2乗するよりベタの方が速い
                    sumB += (pb - ave.B) * (pb - ave.B);
                    sumG += (pg - ave.G) * (pg - ave.G);
                    sumR += (pr - ave.R) * (pr - ave.R);
                    sumY += (py - aveY) * (py - aveY);
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
