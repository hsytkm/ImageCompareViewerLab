using System.Drawing;
using System.Drawing.Imaging;
using ThosoImage;

namespace ThosoImage.Drawing
{
    internal static class GamutReaderImplement
    {
        internal static Gamut ReadGamutRgb(this Bitmap bitmap, Rectangle rectInput)
        {
            int clip(int val, int min, int max)
            {
                if (val <= min) return min;
                if (val >= max) return max;
                return val;
            }

            // 範囲制限
            var rectX = clip(rectInput.X, 0, bitmap.Width);
            var rectY = clip(rectInput.Y, 0, bitmap.Height);
            var rect = new Rectangle(rectX, rectY,
                clip(rectInput.Width, 0, bitmap.Width - rectX),
                clip(rectInput.Height, 0, bitmap.Height - rectY));

            return ProcessUsingLockbitsAndUnsafe(bitmap, ref rect);
        }

        private static Gamut ProcessUsingLockbitsAndUnsafe(Bitmap bitmap, ref Rectangle rect)
        {
            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;

            ulong sumB = 0, sumG = 0, sumR = 0;
            unsafe
            {
                var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
                var stride = bitmapData.Stride;
                var ptrSt = (byte*)bitmapData.Scan0 + rect.Y * stride;
                var ptrEd = ptrSt + rect.Height * stride;
                var xEd = rect.Width * bytesPerPixel;
                for (byte* pixels = ptrSt; pixels < ptrEd; pixels += stride)
                {
                    for (int x = 0; x < xEd; x += bytesPerPixel)
                    {
                        sumB += pixels[x];
                        sumG += pixels[x + 1];
                        sumR += pixels[x + 2];
                    }
                }
                bitmap.UnlockBits(bitmapData);
            }

            var count = (double)(rect.Width * rect.Height);
            var aveR = sumR / count;
            var aveG = sumG / count;
            var aveB = sumB / count;
            return new Gamut(aveR, aveG, aveB);
        }

    }
}
