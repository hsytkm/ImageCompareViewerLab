using ImagePixels.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImagePixels.Drawing
{
    // 残念ながらcore=1よりもcore=4が僅かに早い程度でほぼ同等
    class PixelReader5 : IPixelReader
    {
        public string Name { get; }

        private readonly string ImagePath;
        private readonly int ProcessingCore;

        public PixelReader5(string imagePath, int core)
        {
            ImagePath = imagePath;
            ProcessingCore = core;
            Name = $"Bitmap5(Bitmap2+Parallel{ProcessingCore})";
        }

        public double GetAverageY()
        {
            var imagePath = ImagePath;
            if (!File.Exists(imagePath)) throw new FileNotFoundException();

            int core = ProcessingCore;
            using (var bitmap = new Bitmap(imagePath))
            {
                // 対象領域を分割
                int resolution = bitmap.Height / core;
                var rects = new Rectangle[core];
                for (int i = 0; i < rects.Length - 1; i++)
                {
                    rects[i] = new Rectangle(0, resolution * i, bitmap.Width, resolution);
                }
                rects[core - 1] = new Rectangle(0, resolution * (core - 1), bitmap.Width, bitmap.Height - resolution * (core - 1));

                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

                var rgb = Task.WhenAll(rects.Select(async x =>
                    await ProcessUsingLockbitsAndUnsafe(bitmapData, bytesPerPixel, x))).Result;

                bitmap.UnlockBits(bitmapData);
                return rgb.Select(x => Gamut.GetY(x.R, x.G, x.B)).Average();
            }
        }

        private static async Task<(double R, double G, double B)>
            ProcessUsingLockbitsAndUnsafe(BitmapData bitmapData, int bytesPerPixel, Rectangle rect)
        {
            int heightInPixels = rect.Height;
            int widthInBytes = rect.Width * bytesPerPixel;
            ulong sumB = 0, sumG = 0, sumR = 0;

            await Task.Run(() =>
            {
                unsafe
                {
                    var ptrFirstPixel = (byte*)bitmapData.Scan0 + rect.Y * bitmapData.Stride;
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

            var count = (double)(rect.Width * rect.Height);
            var aveR = sumR / count;
            var aveG = sumG / count;
            var aveB = sumB / count;
            return (aveR, aveG, aveB);
        }

    }
}
