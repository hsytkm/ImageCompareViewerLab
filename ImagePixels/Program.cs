using ImagePixels.BitmapSource;
using ImagePixels.Drawing;
using System;
using System.Collections.Generic;
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

            Console.WriteLine("Start");
            var times = new List<(string name, double Y, TimeSpan ts)>();
            var sw = new Stopwatch();
            string tag;
            double y = 0d;

            // BitmapImage
            tag = "BitmapImage";
            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                y = path.ToBitmapImage().GetAllAverageY();
            }
            times.Add((tag, y, sw.Elapsed));
            Console.WriteLine($"Complete: {tag}");


            // Bitmap1
            tag = "Bitmap1(Lockbits)";
            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                y = path.GetAverageYBitmap1();
            }
            times.Add((tag, y, sw.Elapsed));
            Console.WriteLine($"Complete: {tag}");


            // Bitmap2
            tag = "Bitmap2(Lockbits&Unsafe)";
            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                y = path.GetAverageYBitmap2();
            }
            times.Add((tag, y, sw.Elapsed));
            Console.WriteLine($"Complete: {tag}");


            // 処理時間の出力
            var baseTime = (double)times[0].ts.TotalMilliseconds;
            foreach (var (name, Y, ts) in times)
            {
                Console.WriteLine($"{name,-25}: Y={Y:f2} Time={ts} Ratio={(ts.TotalMilliseconds / baseTime * 100):f1}%");
            }

            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
