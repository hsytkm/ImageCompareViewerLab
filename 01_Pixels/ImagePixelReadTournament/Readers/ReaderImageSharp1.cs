using ImagePixelReadTournament.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace ImagePixelReadTournament.Drawing
{
    class ReaderImageSharp1 : IPixelReader
    {
        public string Name { get; } = "ReaderImageSharp1(UsingImageSharp)";

        private readonly string ImagePath;

        public ReaderImageSharp1(string imagePath)
        {
            ImagePath = imagePath;
        }
        public double GetAverageY()
        {
            var path = ImagePath;
            using (Image<Rgba32> image = Image.Load(path))
            {
                var width = image.Width;
                var height = image.Height;

                ulong sumR = 0, sumG = 0, sumB = 0;
                for (int y = 0; y < height; y++)
                {
                    foreach (var pixel in image.GetPixelRowSpan(y))
                    {
                        sumB += pixel.B;
                        sumG += pixel.G;
                        sumR += pixel.R;
                    }
                }
                var count = (double)(width * height);
                var aveR = sumR / count;
                var aveG = sumG / count;
                var aveB = sumB / count;
                return Gamut.GetY(R: aveR, G: aveG, B: aveB);
            }
        }

    }
}
