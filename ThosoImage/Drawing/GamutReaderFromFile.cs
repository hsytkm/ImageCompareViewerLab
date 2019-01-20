using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ThosoImage;

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

        //private static IReadOnlyList<GamutRgb> GetPixelAverage(this string imagePath, IReadOnlyList<Rectangle> rects)
        //{
        //    if (!File.Exists(imagePath)) throw new FileNotFoundException();
        //    try
        //    {
        //        var gamuts = new List<GamutRgb>(rects.Count);
        //        using (var bitmap = new Bitmap(imagePath))
        //        {
        //            foreach(var rect in rects)
        //            {
        //                gamuts.Add(bitmap.ReadGamutRgb(rect));
        //            }
        //        }
        //        return gamuts;
        //    }
        //    catch (Exception) { throw; }
        //}

    }
}
