using MetadataExtractor;

namespace ImageMetaExtractor.Common
{
    static class MetadataDirectoryExtensions
    {
        /// <summary>
        /// MetadataExtractor.Dictionary から指定IDの情報を取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directory"></param>
        /// <param name="tagType"></param>
        /// <returns></returns>
        public static T GetTagValue<T>(this Directory directory, int tagType) where T : struct
            => (T)directory.GetObject(tagType);

    }
}
