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
            Console.WriteLine($"Start: {tag}");
            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                y = path.ToBitmapImage().GetAllAverageY();
            }
            times.Add((tag, y, sw.Elapsed));

#if false
            // Bitmap1
            tag = "Bitmap1(Lockbits)";
            Console.WriteLine($"Start: {tag}");
            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                y = path.GetAverageYBitmap1();
            }
            times.Add((tag, y, sw.Elapsed));
#endif

#if true
            // Bitmap2
            tag = "Bitmap2(Lockbits&Unsafe)";
            Console.WriteLine($"Start: {tag}");
            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                y = path.GetAverageYBitmap2();
            }
            times.Add((tag, y, sw.Elapsed));
#endif

#if false
            // Bitmap3
            tag = "Bitmap3(Lockbits&Unsafe&Palallel)";
            Console.WriteLine($"Start: {tag}");
            Console.WriteLine("排他制御を行ってないので計算結果が不正になる");
            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                y = path.GetAverageYBitmap3();
            }
            times.Add((tag, y, sw.Elapsed));
#endif

#if false
            // Bitmap4
            tag = "Bitmap4(Lockbits&Unsafe&Span)";
            Console.WriteLine($"Start: {tag}");
            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                y = path.GetAverageYBitmap4();
            }
            times.Add((tag, y, sw.Elapsed));
#endif

#if false
            // Bitmap5
            tag = "Bitmap5(Lockbits&Unsafe&Task)";
            Console.WriteLine($"Start: {tag}");
            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                y = path.GetAverageYBitmap5();
            }
            times.Add((tag, y, sw.Elapsed));
#endif

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
