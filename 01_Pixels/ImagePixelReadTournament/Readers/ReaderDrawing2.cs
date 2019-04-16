using ImagePixelReadTournament.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImagePixelReadTournament.Drawing
{
    class ReaderDrawing2 : IPixelReader
    {
        public string Name { get; } = "Bitmap2(Lockbits&Unsafe)";

        private readonly string ImagePath;

        public ReaderDrawing2(string imagePath)
        {
            ImagePath = imagePath;
        }

        public double GetAverageY()
        {
            var imagePath = ImagePath;
            if (!File.Exists(imagePath)) throw new FileNotFoundException();

            using (var bitmap = new Bitmap(imagePath))
            {
                var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var (R, G, B) = ProcessUsingLockbitsAndUnsafe(bitmap, ref rect);
                return Gamut.GetY(R, G, B);
            }
        }

        private static (double R, double G, double B)
            ProcessUsingLockbitsAndUnsafe(Bitmap bitmap, ref Rectangle rect)
        {
            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var stride = bitmapData.Stride;

            ulong sumB = 0, sumG = 0, sumR = 0;
            try
            {
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
            }
            finally { bitmap.UnlockBits(bitmapData); }

            var count = (double)(rect.Width * rect.Height);
            var aveR = sumR / count;
            var aveG = sumG / count;
            var aveB = sumB / count;
            return (aveR, aveG, aveB);
        }

    }
}
