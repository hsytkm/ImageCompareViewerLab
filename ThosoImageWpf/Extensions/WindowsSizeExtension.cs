using System;
using System.Windows;

namespace ThosoImage.Wpf.Extensions
{
    public static class WindowsSizeExtension
    {
        /// <summary>
        /// Size型の縦横入れ替え
        /// </summary>
        /// <param name="source">入れ替え前のSize</param>
        /// <returns>入れ替え後のSize</returns>
        public static Size Swap(this Size source)
        {
            return new Size(source.Height, source.Width);
        }

        /// <summary>
        /// Size型のメンバを除算
        /// </summary>
        /// <param name="source">処理前のSize</param>
        /// <param name="div">除算値</param>
        /// <returns>処理後のSize</returns>
        public static Size Div(this Size source, double div)
        {
            if (double.IsNaN(div) || div == 0) throw new ArgumentException(nameof(div));
            return new Size(source.Width / div, source.Height / div);
        }

        /// <summary>
        /// Size同士の除算
        /// </summary>
        /// <param name="size1">Size分子</param>
        /// <param name="size2">Size分母</param>
        /// <returns>計算後のSize</returns>
        public static Size Div(this Size size1, Size size2)
        {
            if (double.IsNaN(size2.Width) || size2.Width == 0) throw new ArgumentException(nameof(size2.Width));
            if (double.IsNaN(size2.Height) || size2.Height == 0) throw new ArgumentException(nameof(size2.Height));
            return new Size(size1.Width / size2.Width, size1.Height / size2.Height);
        }

        /// <summary>
        /// Size型のメンバ値制限(幅/高さともに同じ値で制限)
        /// </summary>
        /// <param name="source">制限前のSize</param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// <returns>制限後のSize</returns>
        public static Size Limit(this Size source, double min, double max)
        {
            if (double.IsNaN(min)) throw new ArgumentException(nameof(min));
            if (double.IsNaN(max)) throw new ArgumentException(nameof(max));

            double impl(double val, double mi, double ma)
            {
                if (val <= mi) return mi;
                if (val >= ma) return ma;
                return val;
            }
            return new Size(
                impl(source.Width, min, max),
                impl(source.Height, min, max));
        }

    }
}
