using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageMetaExtractorApp.Models
{
    /// <summary>
    /// メタ情報(全項目表示付き)
    /// </summary>
    class ImageMetasWithAll : ImageMetas
    {
        // 全項目タブ名
        private readonly static string AllGroupName = "All";

        private ImageMetasWithAll(IList<MetaItemGroup> metaItemGroups)
            : base(metaItemGroups) { }

        // インスタンス取得
        public static ImageMetasWithAll GetInstance(string imagePath, IList<MetaItemGroup> oldGroups = null)
        {
            var metaItemGroups = GetMetaItemGroupList(imagePath, oldGroups);

            // 全項目グループを作成して先頭に挿入する
            var metaItems = metaItemGroups.Select(x => x.Items).SelectMany(x => x);
            metaItemGroups.Insert(0, new MetaItemGroup(AllGroupName, metaItems));

            return new ImageMetasWithAll(metaItemGroups);
        }

        // 全項目グループ判定
        public static bool IsAllGroup(MetaItemGroup group) =>
            group?.Name == AllGroupName;

    }
}
