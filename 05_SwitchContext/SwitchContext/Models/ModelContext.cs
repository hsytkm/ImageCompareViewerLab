namespace SwitchContext.Models
{
    class ModelContext
    {
        public static ModelContext Instance { get; } = new ModelContext();
        private ModelContext() { }

        public MainImages MainImages { get; } = new MainImages();

    }
}
