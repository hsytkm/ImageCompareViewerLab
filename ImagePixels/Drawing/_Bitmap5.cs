using ImagePixels.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImagePixels.Drawing
{
    // Bitmap2 + Parallel
    static class Bitmap5
    {
        public static double GetAverageYBitmap5(this string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException();

            int core = 4;
            using (var bitmap = new Bitmap(imagePath))
            {
                int resolution = bitmap.Height / core;
                var rects = new Rectangle[core];
                for (int i = 0; i < rects.Length - 1; i++)
                {
                    rects[i] = new Rectangle(0, resolution * i, bitmap.Width, resolution);
                }
                rects[core - 1] = new Rectangle(0, resolution * (core - 1), bitmap.Width, bitmap.Height - resolution * (core - 1));

                //var rgby = Task.Run(() => rects.Select(x => bitmap.ProcessUsingLockbitsAndUnsafe(ref x))).Result;
                var rgby = Task.WhenAll(rects.Select(async x => await bitmap.ProcessUsingLockbitsAndUnsafe(x))).Result;
                return rgby.Select(x => x.Y).Average();
            }
        }

        private static async Task<(double R, double G, double B, double Y)>
            ProcessUsingLockbitsAndUnsafe(this Bitmap bitmap, Rectangle rect)
        {
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            ulong sumB = 0, sumG = 0, sumR = 0;

            await Task.Run(() =>
            {
                unsafe
                {
                    var ptrFirstPixel = (byte*)bitmapData.Scan0;
                    for (byte* pixels = ptrFirstPixel;
                         pixels < ptrFirstPixel + heightInPixels * bitmapData.Stride;
                         pixels += bitmapData.Stride)
                    {
                        for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                        {
                            sumB += pixels[x];
                            sumG += pixels[x + 1];
                            sumR += pixels[x + 2];
                        }
                    }
                }
            });
            bitmap.UnlockBits(bitmapData);

            var count = (double)(bitmapData.Width * bitmapData.Height);
            var aveR = sumR / count;
            var aveG = sumG / count;
            var aveB = sumB / count;
            var aveY = Gamut.GetY(aveR, aveG, aveB);
            return (aveR, aveG, aveB, aveY);
        }

    }
}
