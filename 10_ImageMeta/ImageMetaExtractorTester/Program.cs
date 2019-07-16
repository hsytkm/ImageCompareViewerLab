using System;
using ImageMetaExtractor;

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

            var me = new MetaExtractor(imagePath);

            Console.WriteLine("Finish.");
        }
    }
}
