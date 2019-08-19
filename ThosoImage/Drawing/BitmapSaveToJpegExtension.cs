using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace ThosoImage.Drawing
{
    public static class BitmapSaveToJpegExtension
    {
        /// <summary>
        /// 画像をJPEGの最高画質で保存
        /// </summary>
        /// <param name="image">保存対象画像</param>
        /// <param name="savePath">保存先</param>
        public static void SaveHighQualityJpeg(this Image image, string savePath)
        {
            try
            {
                SaveQualityJpeg(image, savePath, 100);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 画像をJPEGの指定画質で保存
        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/533jpgquality/jpgquality.html
        /// </summary>
        /// <param name="image">保存対象画像</param>
        /// <param name="savePath">保存先</param>
        /// <param name="quality">品質(0~100)</param>
        public static void SaveQualityJpeg(this Image image, string savePath, long quality)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (savePath is null) throw new ArgumentNullException(nameof(savePath));

            if (quality < 0) quality = 0;
            else if (quality > 100) quality = 100;

            // JPEG用エンコーダ
            var jpgEncoder = ImageCodecInfo.GetImageEncoders()
                .Where(x => x.FormatID == ImageFormat.Jpeg.Guid)
                .FirstOrDefault();
            if (jpgEncoder is null) throw new Exception("Not Found Jpeg Encorder");

            // エンコードパラメータ
            var encParams = new EncoderParameters(1);
            encParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            image.Save(savePath, jpgEncoder, encParams);
        }

    }
}
