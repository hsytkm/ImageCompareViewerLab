using ImagePixels.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ImagePixels.Drawing
{
    // Fast Image Processing in C# http://csharpexamples.com/fast-image-processing-c/
    static class Bitmap3
    {
        public static double GetAverageYBitmap3(this string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException();

            using (var bitmap = new Bitmap(imagePath))
            {
                return bitmap.ProcessUsingLockbitsAndUnsafeAndParallel().Y;
            }
        }

        private static (double R, double G, double B, double Y)
             ProcessUsingLockbitsAndUnsafeAndParallel(this Bitmap processedBitmap)
        {
            var rect = new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height);
            var bitmapData = processedBitmap.LockBits(rect, ImageLockMode.ReadWrite, processedBitmap.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;

            // 排他制御を行っていません
            ulong sumB = 0, sumG = 0, sumR = 0;
            unsafe
            {
                var PtrFirstPixel = (byte*)bitmapData.Scan0;
                Parallel.For(0, heightInPixels, y =>
                {
                    byte* pixels = PtrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                    {
                        sumB += pixels[x];
                        sumG += pixels[x + 1];
                        sumR += pixels[x + 2];
                    }
                });
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
