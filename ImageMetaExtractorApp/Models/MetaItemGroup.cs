using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Meta = ImageMetaExtractor;

namespace ImageMetaExtractorApp.Models
{
    /// <summary>
    /// メタ情報(EXIF, MNoteなど)
    /// </summary>
    class MetaItemGroup
    {
        public string Name { get; }
        public IList<MetaItem> Items { get; }

        public MetaItemGroup(Meta.MetaItemList metaItems)
        {
            if (metaItems is null) throw new ArgumentNullException(nameof(metaItems));

            Name = metaItems.Name;
            Items = metaItems.Select(x => new MetaItem(x)).ToList();
        }

        // 全マークをクリアする
        public void ClearAllMarking() =>
            (Items as List<MetaItem>).ForEach(x => x.ClearMarking());
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

        public void AddMarking() => IsMarking = true;
        public void ClearMarking() => IsMarking = false;
        public void SwitchMark() => IsMarking = !IsMarking;

        public override string ToString() =>
            $"MetaItem: Id={Id}, Key={Key}, Value={Value}";
    }
}
