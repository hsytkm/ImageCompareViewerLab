using System.Collections.Generic;

namespace SwitchContext.Models
{
    class ModelContext
    {
        public static ModelContext Instance { get; } = new ModelContext();
        private ModelContext() { }

        private static readonly string ImagePath1 = @"C:/data/image1.jpg";
        private static readonly string ImagePath2 = @"C:/data/image2.jpg";

        public IList<MainImage> MainImages = new List<MainImage>()
        {
            new MainImage(ImagePath1),
            new MainImage(ImagePath2),
        };
    }
}
