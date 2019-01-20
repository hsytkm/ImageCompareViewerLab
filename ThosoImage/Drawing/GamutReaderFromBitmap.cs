using System;
using System.Drawing;
using ThosoImage;

namespace ThosoImage.Drawing
{
    public static class GamutReaderFromBitmap
    {
        public static Gamut GetPixelAverage(this Bitmap bitmap, Rectangle rect)
        {
            try
            {
                return bitmap.ReadGamutRgb(rect);
            }
            catch (Exception) { throw; }
        }

    }
}
