using System;
using System.Collections.Generic;
using System.IO;

namespace ImageMetaExtractor.Reader
{
    static class ReaderFactory
    {
        public static IImageMeta GetInstance(string imagePath)
        {
            var extension = Path.GetExtension(imagePath).ToLower();
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return new JpegMeta(imagePath);

                case ".bmp":
                    return new BitmapMeta(imagePath);

                case ".png":
                    return new PngMeta(imagePath);

                case ".tif":
                case ".tiff":
                    return new TiffMeta(imagePath);

                case ".gif":
                case ".rw2":
                    return null;
            }

            return null;
        }

    }
}
