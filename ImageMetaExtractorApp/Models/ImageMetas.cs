using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Meta = ImageMetaExtractor;

namespace ImageMetaExtractorApp.Models
{
    class ImageMetas
    {
        // メタ情報リストのリスト(Exif/Mnoteなどがまとめられている)
        public IList<MetaItemGroup> MetaItemGroups { get; private set; }

        public ImageMetas() { }

        public void Load(string imagePath)
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
            var newGroups = libMetaLists.Select(x => new MetaItemGroup(x)).ToList();
            var oldGroups = MetaItemGroups;

            CopyMarking(destGroups: newGroups, srcGroups: oldGroups);
            MetaItemGroups = newGroups;
        }

        // マーキングをコピーする
        private void CopyMarking(IList<MetaItemGroup> destGroups, IList<MetaItemGroup> srcGroups)
        {
            if (destGroups == null || srcGroups == null) return;
            foreach (var srcGroup in srcGroups)
            {
                var srcMarks = srcGroup.Items.Where(x => x.IsMarking);
                if (!srcMarks.Any()) break;     // マークなし

                var destGroup = destGroups.FirstOrDefault(x => x.Name == srcGroup.Name);
                if (destGroup == null) break;   // 同一の名前なし(EXIFとか)

                foreach (var srcItem in srcMarks)
                {
                    var destItem = destGroup.Items.FirstOrDefault(x => x.Id == srcItem.Id);
                    destItem?.Marking(); 
                }
            }
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
    class MetaItem : BindableBase
    {
        public int Id { get; }
        public string Key { get; }
        public string Value { get; }
        public string Comment { get; }

        public bool IsMarking
        {
            get => _IsMarking;
            private set => SetProperty(ref _IsMarking, value);
        }
        private bool _IsMarking;

        public MetaItem(Meta.MetaItem item)
        {
            Id = item.Id;
            Key = item.Key;
            Value = item.Value;
            Comment = item.Comment;
            IsMarking = false;
        }

        public void Marking() => IsMarking = true;

        public void SwitchMark() => IsMarking = !IsMarking;

        public override string ToString() =>
            $"MetaItem: Id={Id}, Key={Key}, Value={Value}";
    }
}
