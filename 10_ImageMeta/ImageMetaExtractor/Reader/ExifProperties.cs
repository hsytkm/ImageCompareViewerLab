using ImageMetaExtractor.Common;

namespace ImageMetaExtractor.Reader
{
    class ExifProperties
    {
        public static readonly string MainTagName = "Exif IFD0";
        public static readonly string SubTagName = "Exif SubIFD";

        // EXIFタグ番号
        public enum EXIF_MAIN_ID : int
        {
            IMAGE_WIDTH = 0x0100,
            IMAGE_HEIGHT = 0x0101,
            MAKER = 0x010f,
            MODEL = 0x0110,
        };

        private readonly MetaItemList _exifItemList;

        public ExifProperties(MetaItemList exifItemList)
        {
            _exifItemList = exifItemList;
        }

        #region メタ情報

        private string GetMetaItemValueString(EXIF_MAIN_ID id)
            => _exifItemList.GetMetaItem((int)id).Value.Trim();

        public string Maker
        {
            get
            {
                if (_maker == null) _maker = GetMetaItemValueString(EXIF_MAIN_ID.MAKER);
                return _maker;
            }
        }
        private string _maker;

        public string Model
        {
            get
            {
                if (_model == null) _model = GetMetaItemValueString(EXIF_MAIN_ID.MODEL);
                return _model;
            }
        }
        private string _model;

        #endregion

    }
}
