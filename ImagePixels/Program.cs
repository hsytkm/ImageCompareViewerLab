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
            double y = 0d;

            var readers = new IPixelReader[]
            {
                new BitmapImageEx(ImagePath),
                new PixelReader1(ImagePath),
                new PixelReader2(ImagePath),
                //new PixelReader3(ImagePath),   // バグってます
                new PixelReader4(ImagePath),
            };

            foreach (var reader in readers)
            {
                Console.WriteLine($"Start: {reader.Name}");
                sw.Restart();
                for (var i = 0; i < count; i++)
                {
                    y = reader.GetAverageY();
                }
                times.Add((reader.Name, y, sw.Elapsed));
            }

            // 処理時間の出力
            var baseTime = times[0].ts.TotalMilliseconds;
            foreach (var (name, Y, ts) in times)
            {
                Console.WriteLine($"{name,-35}: Y={Y:f2} Time={ts} Ratio={(ts.TotalMilliseconds / baseTime * 100):f1}%");
            }

            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
