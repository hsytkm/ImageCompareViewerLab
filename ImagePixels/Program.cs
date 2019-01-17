using ImagePixels.BitmapSource;
using System;
using System.Diagnostics;
using System.IO;

namespace ImagePixels
{
    class Program
    {
        private static readonly string ImagePath = @"C:\data\image1.jpg";

        static void Main(string[] args)
        {
            var path = ImagePath;
            if (!File.Exists(path)) throw new Exception();

            var count = 10;
            double y = 0d;

            var sw = new Stopwatch();

            // BitmapImage
            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                y = path.ToBitmapImage().GetAllAverageY();
            }
            sw.Stop();
            Console.WriteLine($"BitmapImage: Y={y:f2}  {sw.Elapsed}");

            // Drawing

            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
