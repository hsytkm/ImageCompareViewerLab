using System;
using System.Drawing;
using System.IO;

namespace ThosoImage.Drawing
{
    public static class GamutReaderFromFile
    {
        public static Gamut GetAllPixelAverage(this string imagePath)
        {
            try
            {
                var rect = new Rectangle(0, 0, int.MaxValue, int.MaxValue);
                return imagePath.GetPixelAverage(rect);
            }
            catch (Exception) { throw; }
        }

        public static Gamut GetPixelAverage(this string imagePath, int x, int y, int width, int height)
        {
            try
            {
                var rect = new Rectangle(x, y, width, height);
                return imagePath.GetPixelAverage(rect);
            }
            catch (Exception) { throw; }
        }

        private static Gamut GetPixelAverage(this string imagePath, Rectangle rect)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException();
            try
            {
                using (var bitmap = new Bitmap(imagePath))
                {
                    return bitmap.ReadGamutRgb(rect);
                }
            }
            catch (Exception) { throw; }
        }

    }
}
