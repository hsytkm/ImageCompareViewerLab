using Prism.Mvvm;

namespace ImageMetaExtractorApp.Models
{
    class ModelMaster : BindableBase
    {
        // メタ情報クラス
        public ImageMetasFav ImageMetas
        {
            get => _ImageMetas;
            private set => SetProperty(ref _ImageMetas, value);
        }
        private ImageMetasFav _ImageMetas;

        public ModelMaster() { }

        // 引数ファイルPATHからメタ情報クラスを作成
        public void UpdateImage(string filePath) =>
            ImageMetas = ImageMetasFav.GetInstance(filePath, ImageMetas?.MetaItemGroups);

        // メタ情報クラスからマークを全削除
        public void ClearAllMarks() => ImageMetas?.ClearAllMarking();

    }
}
