using ImageMetaExtractor.Common;
using System.Collections.Generic;

namespace ImageMetaExtractor.Reader
{
    /// <summary>
    /// 画像メタ情報のインターフェース
    /// </summary>
    public interface IImageMeta
    {
        string ImagePath { get; }
        int Width { get; }
        int Height { get; }

        bool HasExifMeta { get; }

        /// <summary>
        /// Fileリストの読み出し
        /// </summary>
        /// <returns></returns>
        MetaItemList GetFileMetaItemList();

        /// <summary>
        /// Exifリストグループの読み出し
        /// </summary>
        /// <returns></returns>
        IList<MetaItemList> GetExifMetaListGroup();

    }
}
