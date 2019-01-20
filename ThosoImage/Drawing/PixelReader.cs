using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ThosoImage.Gamut;

namespace ThosoImage.Drawing
{
    public static class PixelReader
    {
        #region FromFilePath

        public static GamutRgb GetAllPixelAverage(this string imagePath)
        {
            try
            {
                var rect = new Rectangle(0, 0, int.MaxValue, int.MaxValue);
                return imagePath.GetPixelAverage(rect);
            }
            catch (Exception) { throw; }
        }

        public static GamutRgb GetPixelAverage(this string imagePath, int x, int y, int width, int height)
        {
            try
            {
                var rect = new Rectangle(x, y, width, height);
                return imagePath.GetPixelAverage(rect);
            }
            catch (Exception) { throw; }
        }

        private static GamutRgb GetPixelAverage(this string imagePath, Rectangle rect)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException();
            try
            {
                using (var bitmap = new Bitmap(imagePath))
                {
                    return ProcessUsingLockbitsAndUnsafe(bitmap, rect);
                }
            }
            catch (Exception) { throw; }
        }

        #endregion

        private static IReadOnlyList<GamutRgb> GetPixelAverage(this string imagePath, IReadOnlyList<Rectangle> rects)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException();
            try
            {
                var gamuts = new List<GamutRgb>(rects.Count);
                using (var bitmap = new Bitmap(imagePath))
                {
                    foreach(var rect in rects)
                    {
                        gamuts.Add(ProcessUsingLockbitsAndUnsafe(bitmap, rect));
                    }
                }
                return gamuts;
            }
            catch (Exception) { throw; }
        }

        #region FromBitmap

        public static GamutRgb GetPixelAverage(this Bitmap bitmap, Rectangle rect)
        {
            try { return ProcessUsingLockbitsAndUnsafe(bitmap, rect); }
            catch (Exception) { throw; }
        }

        #endregion

        #region ReadPixels

        private static GamutRgb ProcessUsingLockbitsAndUnsafe(Bitmap bitmap, Rectangle rectInput)
        {
            int clip(int val, int min, int max)
            {
                if (val <= min) return min;
                if (val >= max) return max;
                return val;
            }

            var rectX = clip(rectInput.X, 0, bitmap.Width);
            var rectY = clip(rectInput.Y, 0, bitmap.Height);
            var rect = new Rectangle(rectX, rectY,
                clip(rectInput.Width, 0, bitmap.Width - rectX),
                clip(rectInput.Height, 0, bitmap.Height - rectY));
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
            return new GamutRgb(aveR, aveG, aveB);
        }

        #endregion

    }
}
