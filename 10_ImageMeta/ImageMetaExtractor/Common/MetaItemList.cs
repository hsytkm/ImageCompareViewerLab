using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageMetaExtractor.Common
{
    /// <summary>
    /// File/ExifなどMetaItemをまとめたリスト
    /// </summary>
    class MetaItemList : ICollection<MetaItem>
    {
        private readonly IList<MetaItem> _metaItems;

        public string Name { get; }

        public MetaItemList(string name, IEnumerable<MetaItem> metas)
        {
            Name = name;
            _metaItems = metas.ToList();
        }

        public MetaItem GetMetaItem(int id) => _metaItems.FirstOrDefault(i => i.Id == id);

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Name);
            foreach (var meta in _metaItems)
                sb.AppendLine(meta.ToString());
            return sb.ToString();
        }

        #region ICollection<T>

        public int Count => _metaItems.Count;

        public bool IsReadOnly => _metaItems.IsReadOnly;

        public void Add(MetaItem item)
        {
            _metaItems.Add(item);
        }

        public void Clear()
        {
            _metaItems.Clear();
        }

        public bool Contains(MetaItem item)
        {
            return _metaItems.Contains(item);
        }

        public void CopyTo(MetaItem[] array, int arrayIndex)
        {
            _metaItems.CopyTo(array, arrayIndex);
        }

        public bool Remove(MetaItem item)
        {
            return _metaItems.Remove(item);
        }

        public IEnumerator<MetaItem> GetEnumerator()
        {
            return _metaItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _metaItems.GetEnumerator();
        }

        #endregion

    }
}
