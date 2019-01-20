using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ThosoImage.Wpf.Imaging
{
    public static class BitmapImageEx
    {
        // Rectの範囲制限
        private static Int32Rect ClipRect(ref Int32Rect rectInput, int width, int height)
        {
            int clip(int val, int min, int max)
            {
                if (val <= min) return min;
                if (val >= max) return max;
                return val;
            }
            var rectX = clip(rectInput.X, 0, width);
            var rectY = clip(rectInput.Y, 0, height);
            return new Int32Rect(rectX, rectY,
                clip(rectInput.Width, 1, width - rectX),
                clip(rectInput.Height, 1, height - rectY));
        }

        // 指定領域の平均輝度の読み込み
        public static Gamut GetPixelAverage(this BitmapSource bitmap, Int32Rect rectInput)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            if (rectInput.Width * rectInput.Height == 0) throw new ArgumentException("RectArea");

            int pixelsByte = (bitmap.Format.BitsPerPixel + 7) / 8; // bit→Byte変換
            int imageWidth = bitmap.PixelWidth;
            int imageHeight = bitmap.PixelHeight;
            var rect = ClipRect(ref rectInput, imageWidth, imageHeight);
            int rectArea = rect.Width * rect.Height;

            var cb = new CroppedBitmap(bitmap, rect);
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
                    return new Gamut(pixels[0]);
                }
                else
                {
                    return new Gamut(r: pixels[2], g: pixels[1], b: pixels[0]);
                }
            }
            else
            {
                if (pixelsByte <= 1)
                {
                    return GetAverage1ch(pixels, rect.Width * rect.Height, pixelsByte);
                }
                else
                {
                    return GetAverage3ch(pixels, rect.Width * rect.Height, pixelsByte);
                }
            }
        }

        // 1ch画素の平均値を計算
        private static Gamut GetAverage1ch(byte[] ps, int count, int pixByte)
        {
            ulong sum = 0;
            for (var i = 0; i < ps.Length; i += pixByte)
            {
                sum += ps[i];
            }
            return new Gamut(sum / (double)count);
        }

        // 3ch画素の平均値を計算
        private static Gamut GetAverage3ch(byte[] ps, int count, int pixByte)
        {
            ulong sumB = 0, sumG = 0, sumR = 0;
            for (var i = 0; i < ps.Length; i += pixByte)
            {
                sumB += ps[i];
                sumG += ps[i + 1];
                sumR += ps[i + 2];
            }
            var aveR = sumR / (double)count;
            var aveG = sumG / (double)count;
            var aveB = sumB / (double)count;
            return new Gamut(aveR, aveG, aveB);
        }

    }
}
