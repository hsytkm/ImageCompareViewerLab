using ImageMetaExtractor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageMetaExtractor.Reader
{
    /// <summary>
    /// 画像メタ情報インターフェースの抽象クラス
    /// </summary>
    abstract class ImageMetaBase : IImageMeta
    {
        public string ImagePath { get; }

        private int? _width;
        public int Width
        {
            get
            {
                if (!_width.HasValue) _width = GetImageWidth();
                return _width.Value;
            }
        }

        private int? _height;
        public int Height
        {
            get
            {
                if (!_height.HasValue) _height = GetImageHeight();
                return _height.Value;
            }
        }

        private bool? _hasExifMeta;
        public bool HasExifMeta
        {
            get
            {
                if (!_hasExifMeta.HasValue) _hasExifMeta = GetHasExifMeta();
                return _hasExifMeta.Value;
            }
        }

        public ImageMetaBase(string imagePath)
        {
            if (!File.Exists(imagePath)) return;
            ImagePath = imagePath;
        }

        // 派生クラスで実装される
        internal abstract int GetImageWidth();
        internal abstract int GetImageHeight();
        internal virtual bool GetHasExifMeta() => false;

        /// <summary>
        /// 全リストの読み出し
        /// </summary>
        /// <returns></returns>
        public IList<MetaItemList> GetAllMetaListGroup()
        {
            var list = new List<MetaItemList>() { GetFileMetaItemList() };
            list.AddRange(GetExifMetaListGroup());
            return list;
        }

        /// <summary>
        /// "File"リストの読み出し
        /// </summary>
        /// <returns></returns>
        public MetaItemList GetFileMetaItemList()
        {
            var fileInfo = new FileInfo(ImagePath);

            var items = new List<(string Key, string Value)>
            {
                ("ファイル名", $"{fileInfo.Name}"),
                ("ファイルサイズ", $"{(fileInfo.Length / 1024.0 / 1024.0):F2} MByte"),
                ("撮影日時", $"{fileInfo.LastWriteTime}"),
                ("画像幅", $"{Width} pixel"),
                ("画像高さ", $"{Height} pixel"),
                ("画素数", $"{Width * Height / 1000_000.0:F2} MP"),
            };

            var metas = items.Select((item, index) => new MetaItem(index, item.Key, item.Value));
            return new MetaItemList("File", metas);
        }

        public virtual IList<MetaItemList> GetExifMetaListGroup()
            => throw new NotImplementedException();

    }
}
