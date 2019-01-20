using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace ThosoImage.Drawing
{
    internal static class GamutReaderImplement
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
                clip(rectInput.Width, 0, width - rectX),
                clip(rectInput.Height, 0, height - rectY));
        }

        // 単一エリアの計算
        internal static Gamut ReadGamutRgb(this Bitmap bitmap, Rectangle rectInput)
        {
            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            try
            {
                // 範囲制限
                var rect = ClipRectangle(rectInput, bitmap.Width, bitmap.Height);
                return ProcessUsingLockbitsAndUnsafe(bitmapData, bytesPerPixel, ref rect);
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
                    var rect = ClipRectangle(rectInput, bitmap.Width, bitmap.Height);
                    yield return ProcessUsingLockbitsAndUnsafe(bitmapData, bytesPerPixel, ref rect);
                }
            }
            finally {
                bitmap.UnlockBits(bitmapData);
            }
        }

        // 画素読み出し本体
        private static Gamut ProcessUsingLockbitsAndUnsafe(BitmapData bitmapData, int bytesPerPixel, ref Rectangle rect)
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
            return new Gamut(aveR, aveG, aveB);
        }

    }
}
