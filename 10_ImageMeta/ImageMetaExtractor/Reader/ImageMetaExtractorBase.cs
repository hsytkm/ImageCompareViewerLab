using ImageMetaExtractor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Metadata = MetadataExtractor; // System.IOとDirectoryが被るので別名を付ける

namespace ImageMetaExtractor.Reader
{
    /// <summary>
    /// MetadataExtractor の IReadOnlyList<Directory> を持つ
    /// </summary>
    abstract class ImageMetaExtractorBase : ImageMetaBase
    {
        // MetadataExtractorの読み出し情報
        internal readonly IReadOnlyList<Metadata.Directory> _directories;

        // 情報の有無フラグ
        internal override bool GetHasExifMeta() => ExifProperties != null;

        // メタタグ名(MetadataExtractorが付けた名前, アプリ表示名)
        private static readonly IList<(string Source, string New)> MetaTagNamePairs =
            new List<(string, string)>()
            {
                (ExifProperties.MainTagName, "EXIF 1"),
                (ExifProperties.SubTagName, "EXIF 2"),
                (MnoteProperties.TagName, "Mnote"),
            };

        // Exif/Mnoteなどメタリスト
        private readonly IList<MetaItemList> _metaItemLists = new List<MetaItemList>();

        // Exifの各情報(機種名/F値など)
        public ExifProperties ExifProperties { get; }
        public MnoteProperties MnoteProperties { get; }

        public ImageMetaExtractorBase(string imagePath) : base(imagePath)
        {
            using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _directories = Metadata.ImageMetadataReader.ReadMetadata(stream);
            }

            // PCで加工されたJPEGはEXIFが消えてることがある
            var exif0 = GetMetaItemList(MetaTagNamePairs[0].Source, MetaTagNamePairs[0].New);
            if (exif0 == null) return;
            _metaItemLists.Add(exif0);
            ExifProperties = new ExifProperties(exif0);

            var exif1 = GetMetaItemList(MetaTagNamePairs[1].Source, MetaTagNamePairs[1].New);
            if (exif1 == null) return;
            _metaItemLists.Add(exif1);

            var mnote = GetMetaItemList(MetaTagNamePairs[2].Source, MetaTagNamePairs[2].New);
            if (mnote == null) return;
            _metaItemLists.Add(mnote);
        }

        /// <summary>
        /// 引数文字列を含むDirectoryを返す
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        internal Metadata.Directory GetDirectory(string tagName)
            => _directories.FirstOrDefault(d => d.Name.Contains(tagName));

        /// <summary>
        /// 読込み済みメタリストグループを返す
        /// </summary>
        /// <returns></returns>
        public override IList<MetaItemList> GetExifMetaListGroup() => _metaItemLists;

        /// <summary>
        /// 対象Dictionaryの全要素を取得
        /// </summary>
        /// <param name="sourceTag"></param>
        /// <param name="newTag"></param>
        /// <returns></returns>
        private MetaItemList GetMetaItemList(string sourceTag, string newTag)
        {
            var directory = GetDirectory(sourceTag);
            if (directory == null) return null;

            // MetadataExtractorの情報をMyXML定義に変換して設定
            var metas = directory.Tags.Select(t => GetMetaItemFromLibTag(directory, t)).ToList();
            if (!metas.Any()) return null;

            // ID順にソート
            metas.Sort((a, b) => a.Id - b.Id);

            return new MetaItemList(newTag, metas);
        }

        /// <summary>
        /// MetadataExtractorの情報をMyXML定義に変換して設定
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private static MetaItem GetMetaItemFromLibTag(Metadata.Directory dir, Metadata.Tag tag)
        {
            int id = tag.Type;
            string key = tag.Name;
            string value = tag.Description;
            object obj = dir.GetObject(id);
            return new MetaItem(id, key, value, obj);

            throw new NotImplementedException();
            //return GetResolvedMetaItem(id, name, content, obj);
        }

    }
}
