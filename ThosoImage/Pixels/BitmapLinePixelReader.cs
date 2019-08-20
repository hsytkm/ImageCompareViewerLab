using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ThosoImage.Pixels
{
    public class BitmapLinePixelReader : IDisposable
    {
        private class BitmapPixels : IDisposable
        {
            private readonly Bitmap _bitmap;
            private readonly BitmapData _bitmapData;
            private readonly int _bytesPerPixel;

            public int Width => _bitmap.Width;
            public int Height => _bitmap.Height;

            private unsafe ReadOnlySpan<byte> Pixels =>
                new ReadOnlySpan<byte>((byte*)_bitmapData.Scan0, _bitmap.Height * _bitmapData.Stride);

            public BitmapPixels(Bitmap bitmap)
            {
                _bitmap = bitmap;
                _bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, bitmap.PixelFormat);

                _bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            }

            public (byte R, byte G, byte B) ReadPixel(int width, int height)
            {
                if (_bitmapData is null) return (0, 0, 0);

                var pixels = Pixels;
                var n = height * _bitmapData.Stride + width * _bytesPerPixel;

                return (R: pixels[n + 2], G: pixels[n + 1], B: pixels[n]);
            }

            public void Dispose() => _bitmap?.UnlockBits(_bitmapData);
        }

        private readonly Bitmap BitmapSource;

        public BitmapLinePixelReader(string imagePath)
        {
            BitmapSource = new Bitmap(imagePath);
        }

        public void Dispose() => BitmapSource?.Dispose();

        /// <summary>
        /// 読み込み済み画素値から2点を結ぶ線上の画素値を返します
        /// </summary>
        /// <param name="point1XRatio">開始点Xの割合(0~1)</param>
        /// <param name="point1YRatio">開始点Yの割合(0~1)</param>
        /// <param name="point2XRatio">終了点Xの割合(0~1)</param>
        /// <param name="point2YRatio">終了点Yの割合(0~1)</param>
        /// <returns>RGBの画素値配列(8bit)</returns>
        public ReadOnlySpan<(byte R, byte G, byte B)> GetRgbLineLevels(
            double point1XRatio, double point1YRatio, double point2XRatio, double point2YRatio)
        {
            using (var bitmapPixels = new BitmapPixels(BitmapSource))
            {
                try
                {
                    int limit(int v, int max) => (v <= 0) ? 0 : ((v >= max) ? max : v);

                    int widthMax = bitmapPixels.Width - 1;
                    int heightMax = bitmapPixels.Height - 1;
                    int p1x = limit((int)(point1XRatio * widthMax), widthMax);
                    int p1y = limit((int)(point1YRatio * heightMax), heightMax);
                    int p2x = limit((int)(point2XRatio * widthMax), widthMax);
                    int p2y = limit((int)(point2YRatio * heightMax), heightMax);

                    int diffX = p2x - p1x;
                    int diffY = p2y - p1y;
                    double distance = Math.Sqrt(diffX * diffX + diffY * diffY);

                    var rgbs = new (byte R, byte G, byte B)[(int)distance];
                    for (int i = 0; i < rgbs.Length; ++i)
                    {
                        int x = (int)Math.Floor(p1x + (diffX * i / distance));
                        int y = (int)Math.Floor(p1y + (diffY * i / distance));
                        rgbs[i] = bitmapPixels.ReadPixel(x, y);
                    }
                    return rgbs;
                }
                catch (Exception) { throw; }
            }
        }

    }
}
