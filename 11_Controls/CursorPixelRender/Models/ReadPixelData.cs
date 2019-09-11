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
        public static ReadPixelArea Zero = default;

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
        public static ReadPixelData Invalid = default;

        public readonly PixelColor Color;

#if true
        public readonly double Average;
#else
        public readonly ulong Sum;
        public readonly int Count;
        public double Average => (double)Sum / Count;
#endif
        public readonly int Max;

        /// <summary>
        /// 整数フラグ(Viewの表示精度に使用される)
        /// </summary>
        public readonly bool IsInteger;

        public ReadPixelData(PixelColor color, double ave, int max, bool isInteger = true)
        {
            Color = color;
            Average = ave;
            Max = max;
            IsInteger = isInteger;
        }
    }

}
