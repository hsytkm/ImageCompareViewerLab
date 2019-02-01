using Prism.Mvvm;

namespace OxyPlotInspector.Models
{
    class ModelMaster : BindableBase
    {
        public static ModelMaster Instance { get; } = new ModelMaster();
        private ModelMaster() { }

        public MainImageSource MainImage = new MainImageSource();

        public Histogram Histogram = new Histogram();

    }
}
