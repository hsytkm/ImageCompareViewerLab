using Prism.Mvvm;

namespace ImageMetaExtractorApp.Models
{
    class ModelMaster : BindableBase
    {
        public ImageMetas ImageMetas
        {
            get => _ImageMetas;
            private set => SetProperty(ref _ImageMetas, value);
        }
        private ImageMetas _ImageMetas;

        public ModelMaster() { }

        public void UpdateImage(string filePath) =>
            ImageMetas = ImageMetas.GetInstance(filePath, ImageMetas?.MetaItemGroups);

        public void ClearAllMarks() => ImageMetas?.ClearAllMarking();

    }
}
