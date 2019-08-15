using System;
using ImageMetaExtractor;
using ImageMetaExtractor.Reader;

namespace ImageMetaExtractorTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var imagePath = @"C:\data\ext\Image1.JPG";

            //imagePath = @"C:\data\ext\Image1.GIF";
            //imagePath = @"C:\data\ext\Image1.png";
            //imagePath = @"C:\data\ext\Image1.tif";
            //imagePath = @"C:\data\ext\Image1.BMP";
            //imagePath = @"C:\data\ext\test.JPG";   //Exifなし

            var imageMeta = new MetaExtractor(imagePath).ImageMeta;
            WriteImageMeta(imageMeta);

            Console.WriteLine("Finish.");
        }

        static void WriteImageMeta(IImageMeta imageMeta)
        {
            if (imageMeta is null) return;

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
        }
    }
}
