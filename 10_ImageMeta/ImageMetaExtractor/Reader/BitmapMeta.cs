using ImageMetaExtractor.Common;
using System;

namespace ImageMetaExtractor.Reader
{
    class BitmapMeta : ImageMetaExtractorBase
    {
        private const string TagName = "BMP Header";

        private enum META_TAG_ID : int
        {
            IMAGE_WIDTH = 0x0002,
            IMAGE_HEIGHT = 0x0001,
        };

        // 何度も取得するのでコンストラクタで用意する
        private readonly int _width;
        private readonly int _height;

        public BitmapMeta(string imagePath) : base(imagePath)
        {
            var directory = GetDirectory(TagName);
            if (directory != null)
            {
                _width = directory.GetTagValue<int>((int)META_TAG_ID.IMAGE_WIDTH);
                _height = directory.GetTagValue<int>((int)META_TAG_ID.IMAGE_HEIGHT);
            }
        }

        internal override int GetImageWidth() => _width;
        internal override int GetImageHeight() => _height;

    }
}
