using ImagePixels.Common;
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
        private static readonly int LoopCount = 10;

        static void Main(string[] args)
        {
            var path = File.Exists(ImagePath) ? ImagePath : args[0];
            if (!File.Exists(path)) throw new FileNotFoundException(path);

            Console.WriteLine("Start");
            Console.WriteLine($"LoopCount: {LoopCount}");
            var (Width, Height) = path.GetImageSize();
            Console.WriteLine($"ImageSize: W={Width} H={Height}");

            var times = new List<(string name, double Y, TimeSpan ts)>();
            var sw = new Stopwatch();
            double y = 0d;

            var readers = new IPixelReader[]
            {
                new BitmapImageEx(ImagePath),   // 基準
                new BitmapImageEx(ImagePath),   // 基準(2回目の方がちょい早い気がする)
                //new PixelReader1(ImagePath),
                new PixelReader2(ImagePath),    // 最速候補
                //new PixelReader3(ImagePath),  // バグってます
                //new PixelReader4(ImagePath),
                //new PixelReader5(ImagePath, 2),   // core1*80%
                new PixelReader5(ImagePath, 4),     // core1*71%
                //new PixelReader5(ImagePath, 8),   // core1*70%
            };

            foreach (var reader in readers)
            {
                Console.WriteLine($"Start: {reader.Name}");
                sw.Restart();
                for (var i = 0; i < LoopCount; i++)
                {
                    y = reader.GetAverageY();
                }
                times.Add((reader.Name, y, sw.Elapsed));
            }

            // 処理時間の出力
            var baseTime = times[1].ts.TotalMilliseconds;   // 2回目基準にする
            foreach (var (name, Y, ts) in times)
            {
                Console.WriteLine($"{name,-35}: Y={Y:f2} Time={ts} Ratio={(ts.TotalMilliseconds / baseTime * 100):f1}%");
            }

            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
