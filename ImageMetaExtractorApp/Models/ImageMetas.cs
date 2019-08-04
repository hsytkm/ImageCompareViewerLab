using System;
using System.Collections.Generic;
using System.Linq;
using Meta = ImageMetaExtractor;

namespace ImageMetaExtractorApp.Models
{
    /// <summary>
    /// メタ情報(お気に入り機能付きのクラスを使用するこちにしたので抽象化した)
    /// </summary>
    abstract class ImageMetas
    {
        // 初期表示するタブ名(指定したくなければnullにする)
        public readonly static string InitViewGroupName = "File";

        // メタ情報リストのリスト(Exif/Mnoteなどがまとめられている)
        public IList<MetaItemGroup> MetaItemGroups { get; private set; }

        internal ImageMetas() { }

        internal ImageMetas(IList<MetaItemGroup> metaItemGroups)
        {
            MetaItemGroups = metaItemGroups;
        }

        // ライブラリを使ってメタ情報リストを読み出し(引数がnullでなければマークを引き継ぐ)
        internal static IList<MetaItemGroup> GetMetaItemGroupList(string imagePath, IList<MetaItemGroup> oldGroups = null)
        {
            var newGroups = GetMetaItemGroupsFromFile(imagePath);

            // マーキングのコピー
            if (oldGroups != null)
                CopyMarking(destGroups: newGroups, srcGroups: oldGroups);

            return newGroups;
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
                    var destItem = destGroup.Items.FirstOrDefault(x => srcItem.IsSameMeta(x));
                    destItem?.AddMarking();
                }
            }
        }

        // 全マークをクリアする
        public void ClearAllMarking() =>
            (MetaItemGroups as List<MetaItemGroup>)?.ForEach(x => x.ClearAllMarking());

    }
}
