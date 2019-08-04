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

        private ImageMetas(IList<MetaItemGroup> metaItemGroups)
        {
            MetaItemGroups = metaItemGroups;
        }

        // インスタンス作成(引数がnullでなければマークを引き継ぐ)
        public static ImageMetas GetInstance(string imagePath, IList<MetaItemGroup> oldGroups = null)
        {
            var newGroups = GetMetaItemGroupsFromFile(imagePath);
            if (oldGroups != null)
                CopyMarking(destGroups: newGroups, srcGroups: oldGroups);
            return new ImageMetas(newGroups);
        }

        // ファイルPATHからメタ情報リストを読み出し
        private static IList<MetaItemGroup> GetMetaItemGroupsFromFile(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) throw new ArgumentNullException(nameof(imagePath));

            var imageMeta = new Meta.MetaExtractor(imagePath).ImageMeta;

            // ライブラリの型List
            var libMetaLists = imageMeta.GetAllMetaListGroup();

            // ViewModelの型List
            return libMetaLists.Select(x => new MetaItemGroup(x)).ToList();
        }

        // マーキングをコピーする
        private static void CopyMarking(IList<MetaItemGroup> destGroups, IList<MetaItemGroup> srcGroups)
        {
            if (destGroups is null || srcGroups is null) return;
            foreach (var srcGroup in srcGroups)
            {
                var srcMarks = srcGroup.Items.Where(x => x.IsMarking);
                if (!srcMarks.Any()) continue;      // マークなし

                var destGroup = destGroups.FirstOrDefault(x => x.Name == srcGroup.Name);
                if (destGroup is null) continue;    // 同一の名前なし(EXIFとか)

                foreach (var srcItem in srcMarks)
                {
                    var destItem = destGroup.Items.FirstOrDefault(x => x.Id == srcItem.Id);
                    destItem?.AddMarking(); 
                }
            }
        }

        // 全マークをクリアする
        public void ClearAllMarking() =>
            (MetaItemGroups as List<MetaItemGroup>)?.ForEach(x => x.ClearAllMarking());

    }
}
