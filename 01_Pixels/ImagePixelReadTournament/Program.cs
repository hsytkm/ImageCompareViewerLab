using ImagePixelReadTournament.Common;
using ImagePixelReadTournament.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ImagePixelReadTournament
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
                new ReaderDrawing2(ImagePath),      // 基準
                new ReaderDrawing2(ImagePath),      // 基準(2回目の方がちょい早い気がする)
                new ReaderImageSharp1(ImagePath),   
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
