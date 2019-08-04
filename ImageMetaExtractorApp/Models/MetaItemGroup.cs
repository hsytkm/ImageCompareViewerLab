using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
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
        public ObservableCollection<MetaItem> Items { get; }

        public MetaItemGroup(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
            Items = new ObservableCollection<MetaItem>();
        }

        public MetaItemGroup(Meta.MetaItemList metaItems)
        {
            if (metaItems is null) throw new ArgumentNullException(nameof(metaItems));

            Name = metaItems.Name;
            Items = new ObservableCollection<MetaItem>(metaItems.Select(x => new MetaItem(Name, x)));
        }

        // 全マークをクリアする
        public void ClearAllMarking()
        {
            foreach (var item in Items) item.ClearMarking();
        }
    }

    /// <summary>
    /// 1つのメタ情報(F値、焦点距離など)
    /// </summary>
    class MetaItem : BindableBase
    {
        public string Unit { get; }     // 所属名(Fileとか)
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

        public MetaItem(string unit, Meta.MetaItem item)
        {
            Unit = unit;
            Id = item.Id;
            Key = item.Key;
            Value = item.Value;
            Comment = item.Comment;
            IsMarking = false;
        }

        // 他社のメーカーノートを区別するためKeyも比較する
        public bool IsSameMeta(MetaItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            return Id == item.Id && Key == item.Key;
        }

        public void SetMarking() => IsMarking = true;
        public void ClearMarking() => IsMarking = false;
        public void SwitchMark() => IsMarking = !IsMarking;

        public override string ToString() =>
            $"{nameof(MetaItem)}: Unit={Unit}, Id={Id}, Key={Key}, Value={Value}," +
            $"Comment={Comment}, IsMarking={IsMarking}";
    }
}
