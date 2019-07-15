using ImageMetaExtractor.Common;
using System;

namespace ImageMetaExtractor.Reader
{
    class TiffMeta : ImageMetaExtractorExifBase
    {
        private const string TagName = "Exif IFD0";

        private enum META_TAG_ID : int
        {
            IMAGE_WIDTH = 0x0100,
            IMAGE_HEIGHT = 0x0101,
        };

        // 何度も取得するのでコンストラクタで用意する
        private readonly int _width;
        private readonly int _height;

        public TiffMeta(string imagePath) : base(imagePath)
        {
            var directory = GetDirectory(TagName);
            if (directory != null)
            {
                _width = (int)directory.GetTagValue<uint>((int)META_TAG_ID.IMAGE_WIDTH);
                _height = (int)directory.GetTagValue<uint>((int)META_TAG_ID.IMAGE_HEIGHT);
            }
        }

        internal override int GetImageWidth() => _width;
        internal override int GetImageHeight() => _height;

    }
}
