using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace VirtualizationListItems.Models
{
    class ImageSources : BindableBase
    {
        // Singleton
        public static ImageSources Instance { get; } = new ImageSources();

        private const string DirPath = @"C:\data";

        public ObservableCollection<ImageSource> Sources = new ObservableCollection<ImageSource>();

        private ImageSources()
        {
            Update(DirPath);
        }

        public void Update(string dirPath)
        {
            foreach (var path in GetImages(dirPath))
            {
                Sources.Add(new ImageSource(path));
            }
        }

        private static IEnumerable<string> GetImages(string directoryPath)
        {
            var pat = ".jpg";
            var images = new List<string>();

            // patは "*.jpg" の形式にする
            foreach (var file in new DirectoryInfo(directoryPath).GetFiles($"*{pat}", SearchOption.TopDirectoryOnly))
            {
                // tiff画像が ".tif" と ".tiff" で二重検出されるので完全一致をチェック
                if (Path.GetExtension(file.FullName).ToLower() == pat)
                    images.Add(file.FullName);
            }
            return images.OrderBy(f => f);
        }

    }
}
