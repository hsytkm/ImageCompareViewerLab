using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageMetaExtractorApp.Models
{
    /// <summary>
    /// メタ情報(お気に入り機能付き)
    /// </summary>
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
            ClearFavMetaItemGroup();

            // フィールドのお気に入りリストを更新
            var favItems = metaItemGroup.Items
                .Where(x => x.IsMarking)
                .Select(x => new FavMetaItem(metaItemGroup.Name, x));
            (FavMetaItems as List<FavMetaItem>).AddRange(favItems);

            // お気に入りグループを更新
            UpdateFavMetaItemGroup();
        }

        // 対象グループのお気に入りから外れたアイテムのみ削除する
        // お気に入りタブで解除された場合も解除する
        private void ClearFavMetaItemGroup()
        {
            // foreachのソースIEnumerableをforeach内で削除したらException出るので一旦リスト化
            var removeItems = new List<FavMetaItem>();
            foreach (var favItem in FavMetaItems)
            {
                var srcItem = GetSourceMetaItem(favItem);
                if (srcItem != null && !srcItem.IsMarking)
                    removeItems.Add(favItem);
            }

            foreach (var item in removeItems)
                FavMetaItems.Remove(item);
        }

        // お気に入りリストをViewに反映する
        private void UpdateFavMetaItemGroup()
        {
            // お気に入りを追加する対象グループ
            var favGroupItems = MetaItemGroups.FirstOrDefault(x => x.Name == FavGroupName)?.Items;
            if (favGroupItems is null) return;
            favGroupItems.Clear();

            // 対象グループで新たにお気に入りされたアイテムのみ追加する
            foreach (var favItem in FavMetaItems)
            {
                var srcItem = GetSourceMetaItem(favItem);
                if (srcItem != null && !favGroupItems.Contains(srcItem))
                    favGroupItems.Add(srcItem);
            }
        }

        // お気に入りメタのソースメタを検索して取得する
        private MetaItem GetSourceMetaItem(FavMetaItem favItem) =>
            MetaItemGroups.FirstOrDefault(x => x.Name == favItem.Unit)?.Items
                .FirstOrDefault(x => x.Id == favItem.Id);

        // お気に入りグループ判定
        public static bool IsFavGroup(MetaItemGroup group) =>
            group?.Name == FavGroupName;

    }

    /// <summary>
    /// 1つのメタ情報のお気に入り(F値とか)
    /// </summary>
    class FavMetaItem
    {
        public string Unit { get; }
        public int Id { get; }
        public string Key { get; }

        private FavMetaItem(string unit, int id, string key)
        {
            Unit = unit;
            Id = id;
            Key = key;
        }

        public FavMetaItem(string group, MetaItem item)
            : this(group, item.Id, item.Key) { }

        public override string ToString() =>
            $"{nameof(FavMetaItem)}: Unit={Unit}, Id ={Id}, Key={Key}";
    }
}
