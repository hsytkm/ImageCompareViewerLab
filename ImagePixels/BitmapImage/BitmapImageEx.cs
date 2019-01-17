using ImagePixels.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImagePixels.BitmapSource
{
    static class BitmapImageEx
    {
        // 画像の読み出し
        public static BitmapImage ToBitmapImage(this string imagePath, bool isCanGC = true)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException();

            var bi = new BitmapImage();
            try
            {
                // アプリが画像ファイルを占有しない
                using (var fs = File.Open(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    bi.StreamSource = fs;
                    bi.EndInit();
                }
                bi.Freeze();
                if (bi.Width == 1 && bi.Height == 1) throw new OutOfMemoryException();
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine($"{ex} ({Path.GetFileName(imagePath)})");

                // メモリリーク時はGCしてみる(画像表示されない現象の低減)
                // https://stackoverflow.com/questions/50040087/c-sharp-bitmapimage-width-and-height-equal-1
                if (isCanGC)
                {
                    GC.Collect();                           // アクセス不可能なオブジェクトを除去
                    GC.WaitForPendingFinalizers();          // ファイナライゼーションが終わるまでスレッド待機
                    GC.Collect();                           // ファイナライズされたばかりのオブジェクトに関連するメモリを開放
                    bi = imagePath.ToBitmapImage(false);    // GC禁止でコール
                }
            }
            return bi;
        }

        // 全画面の平均輝度読み込み
        public static double GetAllAverageY(this BitmapImage bmp)
        {
            if (bmp == null) throw new ArgumentNullException(nameof(bmp));
            var rect = new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight);
            return bmp.GetAverageY(ref rect);
        }

        // 指定領域の平均輝度の読み込み
        private static double GetAverageY(this BitmapImage bmp, ref Int32Rect rect)
        {
            if (bmp == null) throw new ArgumentNullException(nameof(bmp));
            if (rect.Width * rect.Height == 0) throw new ArgumentException("RectArea");

            int pixelsByte = (bmp.Format.BitsPerPixel + 7) / 8; // bit→Byte変換
            int imageWidth = bmp.PixelWidth;
            int imageHeight = bmp.PixelHeight;
            int rectX = rect.X;
            int rectY = rect.Y;
            int rectArea = rect.Width * rect.Height;

            // 範囲制限(とりあえずで幅/高さを保つ方針で実装してます)
            if (imageWidth < rectX + rect.Width)
                rectX = imageWidth - rect.Width;
            if (imageHeight < rectY + rect.Height)
                rectY = imageHeight - rect.Height;

            var cb = new CroppedBitmap(bmp, new Int32Rect(rectX, rectY, rect.Width, rect.Height));
            var pixels = new byte[rectArea * pixelsByte];

            try
            {
                cb.CopyPixels(pixels, rect.Width * pixelsByte, 0);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Trace.WriteLine(ex.Message);    // 謎たまに起きる
            }

            // 1画素(カーソル用)の計算
            if (rectArea == 1)
            {
                if (pixelsByte < 3)
                {
                    //return new Gamut1(pixels[0]);
                }
                else
                {
                    //return new Gamut1(pixels[0], pixels[1], pixels[2]);
                }
            }
            else
            {
                if (pixelsByte <= 1)
                {
                    //return new Gamut2(pixels, new Size(rectWidth, rectHeight), pixelsByte);
                }
                else
                {
                    var (R, G, B, Y) = GetAverage(pixels, rect.Width, rect.Height, pixelsByte);
                    //Debug.WriteLine($"RGBY: {R:f1} {G:f1} {B:f1} {Y:f1}");
                    return Y;
                }
            }
            return 0;
        }

        // 画素の平均値を計算
        private static (double R, double G, double B, double Y)
            GetAverage(byte[] pixels, int width, int height, int pixelsByte)
        {
            ulong sumB = 0, sumG = 0, sumR = 0;
            for (var i = 0; i < pixels.Length; i += pixelsByte)
            {
                sumB += pixels[i + 0];
                sumG += pixels[i + 1];
                sumR += pixels[i + 2];
            }
            var count = (double)(width * height);
            var aveR = sumR / count;
            var aveG = sumG / count;
            var aveB = sumB / count;
            var aveY = Gamut.GetY(aveR, aveG, aveB);
            return (aveR, aveG, aveB, aveY);
        }

    }
}
