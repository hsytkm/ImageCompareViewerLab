using ImagePixels.Common;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImagePixels.Drawing
{
    static class Bitmap4
    {
        public static double GetAverageYBitmap4(this string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException();

            using (var bitmap = new Bitmap(imagePath))
            {
                return bitmap.ProcessUsingLockbitsAndSpan().Y;
            }
        }

        // .NETFramework4.6.1のSpanは高速でないとどこかで見た気がする。実際にイマイチやった
        private static (double R, double G, double B, double Y)
            ProcessUsingLockbitsAndSpan(this Bitmap processedBitmap)
        {
            var rect = new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height);
            var bitmapData = processedBitmap.LockBits(rect, ImageLockMode.ReadOnly, processedBitmap.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            ulong sumB = 0, sumG = 0, sumR = 0;

            unsafe
            {
                var ptrFirstPixel = (byte*)bitmapData.Scan0;
                for (byte* y = ptrFirstPixel;
                     y < ptrFirstPixel + heightInPixels * bitmapData.Stride;
                     y += bitmapData.Stride)
                {
                    var pixels = new ReadOnlySpan<byte>(y, widthInBytes);
                    for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                    {
                        sumB += pixels[x];
                        sumG += pixels[x + 1];
                        sumR += pixels[x + 2];
                    }
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
