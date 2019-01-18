using ImagePixels.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ImagePixels.Drawing
{
    // Fast Image Processing in C# http://csharpexamples.com/fast-image-processing-c/
    static class Bitmap1
    {
        public static double GetAverageYBitmap1(this string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException();

            using (var bitmap = new Bitmap(imagePath))
            {
                return bitmap.ProcessUsingLockbits().Y;
            }
        }

        private static (double R, double G, double B, double Y)
            ProcessUsingLockbits(this Bitmap processedBitmap)
        {
            var rect = new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height);
            var bitmapData = processedBitmap.LockBits(rect, ImageLockMode.ReadOnly, processedBitmap.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * processedBitmap.Height;

            var pixels = new byte[byteCount];
            Marshal.Copy(bitmapData.Scan0, pixels, 0, pixels.Length);
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;

            ulong sumB = 0, sumG = 0, sumR = 0;
            for (int y = 0; y < heightInPixels * bitmapData.Stride; y += bitmapData.Stride)
            {
                for (int x = y; x < y + widthInBytes; x += bytesPerPixel)
                {
                    sumB += pixels[x];
                    sumG += pixels[x + 1];
                    sumR += pixels[x + 2];
                }
            }
            processedBitmap.UnlockBits(bitmapData);

            var count = (double)(bitmapData.Width * bitmapData.Height);
            var aveR = sumR / count;
            var aveG = sumG / count;
            var aveB = sumB / count;
            var aveY = Gamut.GetY(aveR, aveG, aveB);
            return (aveR, aveG, aveB, aveY);
        }

    }
}
