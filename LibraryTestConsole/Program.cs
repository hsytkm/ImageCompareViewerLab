using System;
using System.IO;
using ThosoImage.Drawing;

namespace LibraryTestConsole
{
    class Program
    {
        private static readonly string ImagePath = @"C:\data\image1.jpg";

        static void Main(string[] args)
        {
            var path = File.Exists(ImagePath) ? ImagePath : args[0];
            if (!File.Exists(path)) throw new FileNotFoundException(path);

            // 画素平均
            var gamut = path.GetAllPixelAverage();
            Console.WriteLine($"R={gamut.Rgb.R:f2} G={gamut.Rgb.G:f2} B={gamut.Rgb.B:f2}");
            Console.WriteLine($"Y={gamut.Y:f2}");
            Console.WriteLine($"L={gamut.Lab.L:f2} a={gamut.Lab.a:f2} b={gamut.Lab.b:f2}");

            Console.ReadKey();
        }
    }
}
