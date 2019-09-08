using System;

namespace CursorPixelRender.Models
{
    /// <summary>
    /// 画素色
    /// </summary>
    public enum PixelColor
    {
        None, R, G, B, Gr, Gb, Y, L, a, b,
    }

    /// <summary>
    /// 画素の読み出し領域
    /// </summary>
    readonly struct ReadPixelArea
    {
        public static ReadPixelArea Zero = new ReadPixelArea(0, 0, 0, 0);

        public readonly int X;
        public readonly int Y;
        public readonly int Width;
        public readonly int Height;

        public ReadPixelArea(int x, int y, int w, int h) =>
            (X, Y, Width, Height) = (x, y, w, h);
    }

    /// <summary>
    /// 画像から読み出した画素値
    /// </summary>
    readonly struct ReadPixelData
    {
        public static ReadPixelData Invalid = new ReadPixelData(PixelColor.None, 0, 0);

        public readonly PixelColor Color;

        public readonly int Max;

#if true
        public readonly double Average;
#else
        public readonly ulong Sum;
        public readonly int Count;
        public double Average => (double)Sum / Count;
#endif

        public ReadPixelData(PixelColor color, int max, double ave)
        {
            Color = color;
            Max = max;
            Average = ave;
        }
    }

    /// <summary>
    /// 画像から読み出した画素情報
    /// </summary>
    class ReadPixelsData
    {
        public ReadPixelArea ReadArea { get; }
        private readonly ReadPixelData[] _pixels;

        public ReadPixelsData(in ReadPixelArea area, ReadPixelData[] pixels)
        {
            ReadArea = area;
            _pixels = pixels;
        }

        /// <summary>
        /// 指定色の構造体を検索して返す
        /// </summary>
        /// <param name="pixelColor"></param>
        /// <param name="readPixelData"></param>
        /// <returns></returns>
        public bool TryToFindPixelColor(PixelColor pixelColor, out ReadPixelData readPixelData)
        {
            if (_pixels != null)
            {
                foreach (var pixel in _pixels)
                {
                    if (pixel.Color == pixelColor)
                    {
                        readPixelData = pixel;
                        return true;
                    }
                }
            }
            readPixelData = ReadPixelData.Invalid;
            return false;
        }

#if false
        /// <summary>
        /// 指定色を含むか判定
        /// </summary>
        /// <param name="pixelColor"></param>
        /// <returns></returns>
        public bool IsContainPixelColor(PixelColor pixelColor) =>
            TryToFindPixelColor(pixelColor, out var _);

        /// <summary>
        /// 指定色の平均値を取得
        /// </summary>
        /// <param name="pixelColor"></param>
        /// <returns></returns>
        public double GetPixelAverage(PixelColor pixelColor)
        {
            if (TryToFindPixelColor(pixelColor, out var pixel))
            {
                return pixel.Average;
            }
            return default;
        }
#endif

    }

}
