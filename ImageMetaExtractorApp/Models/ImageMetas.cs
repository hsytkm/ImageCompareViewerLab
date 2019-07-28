using System;
using System.Collections.Generic;
using System.Linq;
using Meta = ImageMetaExtractor;

namespace ImageMetaExtractorApp.Models
{
    class ImageMetas
    {
        // メタ情報リストのリスト(Exif/Mnoteなどがまとめられている)
        //public ObservableCollection<MetaItemGroup> MetaItemGroups2 { get; }
        public IEnumerable<MetaItemGroup> MetaItemGroups { get; }

        public ImageMetas(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) throw new ArgumentNullException();

            var imageMeta = new Meta.MetaExtractor(imagePath).ImageMeta;

            // ライブラリ型のList
            var libMetaLists = new List<Meta.MetaItemList>()
            {
                imageMeta.GetFileMetaItemList()
            };
            libMetaLists.AddRange(imageMeta.GetExifMetaListGroup());

            // ViewModel用の型ListのList
            MetaItemGroups = libMetaLists.Select(x => new MetaItemGroup(x));
        }
    }

    /// <summary>
    /// メタ情報(EXIF, MNoteなど)
    /// </summary>
    class MetaItemGroup
    {
        public string Name { get; }
        public IList<MetaItem> Items { get; }

        public MetaItemGroup(Meta.MetaItemList metaItems)
        {
            if (metaItems == null) throw new ArgumentNullException();

            Name = metaItems.Name;
            Items = metaItems.Select(x => new MetaItem(x)).ToList();
        }
    }

    /// <summary>
    /// 1つのメタ情報(F値、焦点距離など)
    /// </summary>
    class MetaItem
    {
        public int Id { get; }
        public string Key { get; }
        public string Value { get; }
        public string Comment { get; }

        public MetaItem(Meta.MetaItem item)
        {
            Id = item.Id;
            Key = item.Key;
            Value = item.Value;
            Comment = item.Comment;
        }
    }
}
