using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace ThosoImage.Wpf.Imaging
{
    public static class BitmapSourceToFile
    {
        // JPEG画質(Min=1, Max=100)
        private const int DefaultQualityLevel = 100;

        /// <summary>
        /// BitmapSourceをファイルに保存する
        /// </summary>
        /// <param name="bitmap">保存する画像</param>
        /// <param name="filePath">保存PATH</param>
        public static void SaveToFile(this BitmapSource bitmap, string filePath, int qualityLevel = DefaultQualityLevel)
        {
            if (bitmap == null) throw new ArgumentNullException();
            if (filePath == null) throw new ArgumentNullException();

            if (qualityLevel < 1) qualityLevel = 1;
            else if (qualityLevel > 100) qualityLevel = 100;

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var encoder = new JpegBitmapEncoder
                {
                    QualityLevel = qualityLevel
                };
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(fileStream);
            }
        }

    }
}
