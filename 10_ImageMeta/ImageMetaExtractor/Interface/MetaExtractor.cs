using System;
using ImageMetaExtractor.Reader;

namespace ImageMetaExtractor
{
    public class MetaExtractor
    {
        public IImageMeta ImageMeta { get; }

        public MetaExtractor(string imagePath)
        {
            var imageMeta = ReaderFactory.GetInstance(imagePath);
            ImageMeta = imageMeta;
            if (imageMeta is null) return;

#if false
            // File
            var fileMetaList = imageMeta.GetFileMetaItemList();
            Console.WriteLine(fileMetaList);

            // Exif/Makernote
            if (imageMeta.HasExifMeta)
            {
                var exifMetaListGroup = imageMeta.GetExifMetaListGroup();
                foreach (var metaList in exifMetaListGroup)
                {
                    Console.WriteLine(metaList);
                }
            }
#endif
        }

    }
}
