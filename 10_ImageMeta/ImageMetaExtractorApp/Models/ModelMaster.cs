using Prism.Mvvm;

namespace ImageMetaExtractorApp.Models
{
    class ModelMaster : BindableBase
    {
        // メタ情報クラス
        public ImageMetasWithAll ImageMetas
        {
            get => _ImageMetas;
            private set => SetProperty(ref _ImageMetas, value);
        }
        private ImageMetasWithAll _ImageMetas;

        public ModelMaster() { }

        // 引数ファイルPATHからメタ情報クラスを作成
        public void UpdateImage(string filePath) =>
            ImageMetas = ImageMetasWithAll.GetInstance(filePath, ImageMetas?.MetaItemGroups);

        // メタ情報クラスからマークを全削除
        public void ClearAllMarks() => ImageMetas?.ClearAllMarking();

    }
}
