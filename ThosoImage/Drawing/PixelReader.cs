using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ThosoImage.Gamut;

namespace ThosoImage.Drawing
{
    public class PixelReader
    {
        private readonly string ImagePath;

        public PixelReader(string imagePath)
        {
            ImagePath = imagePath;
        }

        public GamutRgb GetAllPixelAverage()
        {
            var rect = new Rectangle(0, 0, int.MaxValue, int.MaxValue);
            return GetAllPixelAverage(ref rect);
        }

        public GamutRgb GetAllPixelAverage(int x, int y, int width, int height)
        {
            var rect = new Rectangle(x, y, width, height);
            return GetAllPixelAverage(ref rect);
        }

        public GamutRgb GetAllPixelAverage(ref Rectangle rect)
        {
            var imagePath = ImagePath;
            if (!File.Exists(imagePath)) throw new FileNotFoundException();
            using (var bitmap = new Bitmap(imagePath))
            {
                return ProcessUsingLockbitsAndUnsafe(bitmap, ref rect);
            }
        }

        private static GamutRgb ProcessUsingLockbitsAndUnsafe(Bitmap bitmap, ref Rectangle rectInput)
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
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var stride = bitmapData.Stride;

            ulong sumB = 0, sumG = 0, sumR = 0;
            unsafe
            {
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
            }
            bitmap.UnlockBits(bitmapData);

            var count = (double)(rect.Width * rect.Height);
            var aveR = sumR / count;
            var aveG = sumG / count;
            var aveB = sumB / count;
            return new GamutRgb(aveR, aveG, aveB);
        }

    }
}
