using Meta = ImageMetaExtractor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ImageMetaExtractorApp.Models
{
    class ImageMetas
    {
        // メタ情報リストのリスト(Exif/Mnoteなどがまとめられている)
        public ObservableCollection<ObservableCollection<MetaItem>> MetaItemLists { get; }

        public ImageMetas(string imagePath)
        {
            var imageMeta = new Meta.MetaExtractor(imagePath).ImageMeta;

            // ライブラリ型のList
            var libMetaLists = new List<Meta.MetaItemList>()
            {
                imageMeta.GetFileMetaItemList()
            };
            libMetaLists.AddRange(imageMeta.GetExifMetaListGroup());

            // ViewModel用の型ListのList
            MetaItemLists = new ObservableCollection<ObservableCollection<MetaItem>>(
                libMetaLists.Select(x => ToMetaItemObservableCollection(x)));
        }

        private ObservableCollection<MetaItem> ToMetaItemObservableCollection(Meta.MetaItemList metaItems) =>
            new ObservableCollection<MetaItem>(metaItems.Select(x => new MetaItem(x)));
    }

    /// <summary>
    /// メタ情報
    /// </summary>
    class MetaItem
    {
        public int Id { get; }
        public string Key { get; }
        public string Value { get; }

        public MetaItem(Meta.MetaItem item)
        {
            Id = item.Id;
            Key = item.Key;
            Value = item.Value;
        }

    }
}
