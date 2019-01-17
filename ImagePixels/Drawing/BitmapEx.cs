using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace ImagePixels.Drawing
{
    // Fast Image Processing in C# http://csharpexamples.com/fast-image-processing-c/
    static class BitmapEx
    {
        // 画像の読み出し
        public static Bitmap ToBitmap(this string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException();

            // 未実装

            return null;
        }

        // 全画面輝度を返す
        public static double GetAllAverageY(this Bitmap bmp)
        {
            if (bmp == null) throw new ArgumentNullException(nameof(bmp));
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            return bmp.GetAverageY(ref rect);
        }

        // 指定領域の平均輝度の読み込み
        private static double GetAverageY(this Bitmap bmp, ref Rectangle rect)
        {
            if (bmp == null) throw new ArgumentNullException(nameof(bmp));
            if (rect.Width * rect.Height == 0) throw new ArgumentException("RectArea");

            return 0;
        }

    }
}
