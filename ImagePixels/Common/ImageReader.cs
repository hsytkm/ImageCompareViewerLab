using System;
using System.Drawing;
using System.IO;

namespace ImagePixels.Common
{
    static class ImageReader
    {
        // 画像サイズ取得
        public static (int Width, int Height) GetImageSize(this string path)
        {
            if (!File.Exists(path)) throw new ArgumentException(path);

            try
            {
                // イメージデータの検証を無効にすることで高速化
                using (var stream = File.OpenRead(path))
                using (var image = Image.FromStream(stream, false, false))
                {
                    return (image.Width, image.Height);
                }
            }
            catch (Exception) { throw; }
        }
    }
}
