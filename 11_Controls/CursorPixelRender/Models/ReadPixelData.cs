using System;

namespace CursorPixelRender.Models
{
    public enum PixelColor
    {
        None, R, G, B, Gr, Gb,
    }

    struct ReadPixelData
    {
        public static ReadPixelData Invalid = new ReadPixelData(PixelColor.None, 0, 0);

        public PixelColor Color;
        public int Max;
#if true
        public double Average;
#else
        public ulong Sum;
        public int Count;
        public double Average => (double)Sum / Count;
#endif

        public ReadPixelData(PixelColor color, int max, double ave)
        {
            Color = color;
            Max = max;
            Average = ave;
        }
    }

    class ReadPixelsData
    {
        private readonly ReadPixelData[] _pixels;

        public ReadPixelsData(ReadPixelData[] pixels)
        {
            _pixels = pixels;
        }

        /// <summary>
        /// 指定色の構造体を検索して返す
        /// </summary>
        /// <param name="pixelColor"></param>
        /// <param name="readPixelData"></param>
        /// <returns></returns>
        private bool TryToFindPixelColor(PixelColor pixelColor, out ReadPixelData readPixelData)
        {
            readPixelData = ReadPixelData.Invalid;

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
            return false;
        }

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

    }

}
