using System;
using System.Drawing;
using System.IO;

namespace ThosoImage.ColorSpace
{
    public static class GamutReaderFromFileExtension
    {
        /// <summary>
        /// 引数画像の全画素の平均値を取得する
        /// </summary>
        /// <param name="imagePath">対象画像PATH</param>
        /// <returns>Gamut</returns>
        public static Gamut GetAllPixelAverage(this string imagePath)
        {
            try
            {
                if (imagePath is null) throw new ArgumentNullException();
                var rect = new Rectangle(0, 0, int.MaxValue, int.MaxValue);
                return GetPixelAverage(imagePath, ref rect);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 引数画像の全画素の平均値を取得する
        /// </summary>
        /// <param name="imagePath">対象画像PATH</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>Gamut</returns>
        public static Gamut GetPixelAverage(this string imagePath, int x, int y, int width, int height)
        {
            try
            {
                var rect = new Rectangle(x, y, width, height);
                return GetPixelAverage(imagePath, ref rect);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 引数画像の全画素の平均値を取得する
        /// </summary>
        /// <param name="imagePath">対象画像PATH</param>
        /// <param name="rect"></param>
        /// <returns>Gamut</returns>
        private static Gamut GetPixelAverage(this string imagePath, ref Rectangle rect)
        {
            if (imagePath is null) throw new ArgumentNullException();
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
