using System;
using System.Drawing;
using System.IO;

namespace ThosoImage.Drawing
{
    public static class ImageSize
    {
        /// <summary>
        /// ファイルから画像サイズを取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static (int Width, int Height) GetImageSize(this string path)
        {
            if (!File.Exists(path)) throw new ArgumentException(path);

            try
            {
                // イメージデータの検証を無効化することにより高速化
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
