using System;

namespace ThosoImage.Gamut
{
    public class GamutRgb
    {
        #region RGB

        // 小数(必ず設定される)
        public (double R, double G, double B) Rgb { get; }

        // 整数(1画素から求めたデータなら設定される)
        private (byte R, byte G, byte B)? IntRgb { get; }
        public bool IsInteger { get => IntRgb.HasValue; }

        #endregion

        #region YData

        private double? _Y = null;
        public double Y
        {
            get
            {
                if (!_Y.HasValue)
                    _Y = 0.299 * Rgb.R + 0.587 * Rgb.G + 0.114 * Rgb.B;
                return _Y.Value;
            }
        }

        #endregion

        #region Lab

        private (double L, double a, double b)? _Lab = null;
        public (double L, double a, double b) Lab
        {
            get
            {
                if (!_Lab.HasValue) _Lab = GetLab(Rgb.B, Rgb.G, Rgb.R);
                return _Lab.Value;
            }
        }
        //public double LabL => Lab.L;
        //public double Laba => Lab.a;
        //public double Labb => Lab.b;

        /// <summary>
        /// RGBからLabを計算(Web拾い版) これが正しいか不明…
        /// https://qiita.com/hachisuka_nsw/items/09caabe6bec46a2a0858
        /// </summary>
        /// <param name="blue">Bch(0.0~255.0)</param>
        /// <param name="green">Gch(0.0~255.0)</param>
        /// <param name="red">Rch(0.0~255.0)</param>
        /// <returns></returns>
        private static (double L, double a, double b) GetLab(double blue, double green, double red)
        {
            var rgb_r = red / 255.0;
            var rgb_g = green / 255.0;
            var rgb_b = blue / 255.0;

            rgb_r = (rgb_r > 0.04045) ? Math.Pow(((rgb_r + 0.055) / 1.055), 2.4) : (rgb_r / 12.92);
            rgb_g = (rgb_g > 0.04045) ? Math.Pow(((rgb_g + 0.055) / 1.055), 2.4) : (rgb_g / 12.92);
            rgb_b = (rgb_b > 0.04045) ? Math.Pow(((rgb_b + 0.055) / 1.055), 2.4) : (rgb_b / 12.92);

            var x = (rgb_r * 0.4124) + (rgb_g * 0.3576) + (rgb_b * 0.1805);
            var y = (rgb_r * 0.2126) + (rgb_g * 0.7152) + (rgb_b * 0.0722);
            var z = (rgb_r * 0.0193) + (rgb_g * 0.1192) + (rgb_b * 0.9505);

            x = (x * 100.0) / 95.0470;
            //y = (y * 100.0) / 100.000;
            z = (z * 100.0) / 108.883;

            x = (x > 0.008856) ? Math.Pow(x, 1.0 / 3.0) : (7.787 * x) + (4.0 / 29.0);
            y = (y > 0.008856) ? Math.Pow(y, 1.0 / 3.0) : (7.787 * y) + (4.0 / 29.0);
            z = (z > 0.008856) ? Math.Pow(z, 1.0 / 3.0) : (7.787 * z) + (4.0 / 29.0);

            var labl =(116.0 * y) - 16.0;
            var laba = 500.0 * (x - y);
            var labb = 200.0 * (y - z);
            return (labl, laba, labb);
        }

        #endregion

        public GamutRgb(double r, double g, double b)
        {
            Rgb = (r, g, b);
            IntRgb = null;
        }

        public GamutRgb(byte r, byte g, byte b)
        {
            Rgb = (r, g, b);
            IntRgb = (r, g, b);
        }

        public override string ToString()
        {
            return IntRgb.HasValue ?
                $"R={IntRgb.Value.R} G={IntRgb.Value.G} B={IntRgb.Value.B}" :
                $"R={Rgb.R:f2} G={Rgb.G:f2} B={Rgb.B:f2}";
        }

    }
}
