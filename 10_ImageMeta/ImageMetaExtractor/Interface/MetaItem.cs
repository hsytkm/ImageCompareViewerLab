namespace ImageMetaExtractor
{
    /// <summary>
    /// メタ情報
    /// </summary>
    public struct MetaItem
    {
        public readonly int Id;
        public readonly string Key;
        public readonly string Value;
        public readonly object Content;
        public readonly string Comment;

        public MetaItem(int id, string key, string value, object content = null, string comment = null)
        {
            Id = id;
            Key = key;
            Value = value;
            Content = content;
            Comment = comment;

            // Commentの設定が未実装なので何か入れとく
            if (string.IsNullOrWhiteSpace(Comment)) Comment = ToString();
        }

        public MetaItem(int id, object content, string comment = null)
        {
            Id = id;
            Content = content;
            Comment = comment;

            // 未実装
            Key = "";
            Value = "";
        }

        public override string ToString()
            => $"Id=0x{Id:X4}, Key=\"{Key}\", Value=\"{Value}\", Content={(Content != null)}, Comment=\"{Comment}\"";

    }
}
