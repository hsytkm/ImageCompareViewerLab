using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageMetaExtractorApp.Models
{
    class ImageMetasFav : ImageMetas
    {
        // お気に入りタブ名
        private readonly static string FavGroupName = "Fav";

        // お気に入りメタアイテムのリスト(F値/ISOとか)
        public IList<FavMetaItem> FavMetaItems { get; private set; }

        private ImageMetasFav(IList<MetaItemGroup> metaItemGroups)
            : base(metaItemGroups)
        {
            FavMetaItems = new List<FavMetaItem>();
        }

        public static ImageMetasFav GetInstance(string imagePath, IList<MetaItemGroup> oldGroups = null)
        {
            var metaItemGroups = GetMetaItemGroupList(imagePath, oldGroups);

            // お気に入りグループを先頭に挿入する
            metaItemGroups.Insert(0, new MetaItemGroup(FavGroupName));

            return new ImageMetasFav(metaItemGroups);
        }

        // メタ情報からお気に入りを探して追加する
        public void AddFavMetaItem(MetaItemGroup metaItemGroup)
        {
            // お気に入りから外れたアイテムのみ削除
            ClearFavMetaItemGroup(metaItemGroup);

            // フィールドのお気に入りリストを更新
            var favItems = metaItemGroup.Items
                .Where(x => x.IsMarking)
                .Select(x => new FavMetaItem(metaItemGroup.Name, x));
            (FavMetaItems as List<FavMetaItem>).AddRange(favItems);

            // お気に入りグループを更新
            UpdateFavMetaItemGroup();
        }

        // 対象グループのお気に入りから外れたアイテムのみ削除する
        private void ClearFavMetaItemGroup(MetaItemGroup metaItemGroup)
        {
            // foreachのソースIEnumerableをforeach内で削除したらException出るので一旦リスト化
            var removeItems = new List<FavMetaItem>();
            foreach (var favItem in FavMetaItems)
            {
                var sourceItem = MetaItemGroups.FirstOrDefault(x => x.Name == favItem.Group)?.Items
                    .FirstOrDefault(x => x.Id == favItem.Id);
                if (sourceItem != null && !sourceItem.IsMarking)
                    removeItems.Add(favItem);
            }

            foreach (var item in removeItems)
                FavMetaItems.Remove(item);
        }

        // お気に入りリストを反映する
        private void UpdateFavMetaItemGroup()
        {
            // お気に入りを追加する対象グループ
            var favGroupItems = MetaItemGroups.FirstOrDefault(x => x.Name == FavGroupName)?.Items;
            if (favGroupItems is null) return;
            favGroupItems.Clear();

            // 対象グループで新たにお気に入りされたアイテムのみ追加する
            foreach (var favItem in FavMetaItems)
            {
                var itemGroup = MetaItemGroups.FirstOrDefault(x => x.Name == favItem.Group);
                if (itemGroup is null) continue;

                var item = itemGroup.Items.FirstOrDefault(x => x.Id == favItem.Id);
                if (item is null) continue;

                if (!favGroupItems.Contains(item))
                    favGroupItems.Add(item);
            }
        }

    }

    /// <summary>
    /// 1つのメタ情報のお気に入り(F値とか)
    /// </summary>
    class FavMetaItem
    {
        public string Group { get; }
        public int Id { get; }
        public string Key { get; }

        private FavMetaItem(string group, int id, string key)
        {
            Group = group;
            Id = id;
            Key = key;
        }

        public FavMetaItem(string group, MetaItem item)
            : this(group, item.Id, item.Key) { }

        public override string ToString() =>
            $"{nameof(FavMetaItem)}: Group={Group}, Id ={Id}, Key={Key}";
    }
}
