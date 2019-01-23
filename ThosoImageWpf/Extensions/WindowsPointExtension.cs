using System;
using System.Windows;

namespace ThosoImage.Wpf.Extensions
{
    public static class WindowsPointExtension
    {
        /// <summary>
        /// Point型同士の加算
        /// </summary>
        /// <param name="point1">Point1</param>
        /// <param name="point2">Point2</param>
        /// <returns>処理後のPoint</returns>
        public static Point Plus(this Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }

        /// <summary>
        /// Point型同士の除算
        /// </summary>
        /// <param name="point1">Point分子</param>
        /// <param name="point2">Point分母</param>
        /// <returns>処理後のPoint</returns>
        public static Point Div(this Point point1, Point point2)
        {
            if (double.IsNaN(point2.X) || point2.X == 0) throw new ArgumentException(nameof(point2.X));
            if (double.IsNaN(point2.Y) || point2.Y == 0) throw new ArgumentException(nameof(point2.Y));
            return new Point(point1.X / point2.X, point1.Y / point2.Y);
        }

        /// <summary>
        /// Point型のメンバ値制限(X/Yともに同じ値で制限)
        /// </summary>
        /// <param name="source">制限前のPoint</param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// <returns>制限後のSize</returns>
        public static Point Limits(this Point source, double min, double max)
        {
            if (double.IsNaN(min)) throw new ArgumentException(nameof(min));
            if (double.IsNaN(max)) throw new ArgumentException(nameof(max));

            double impl(double val, double mi, double ma)
            {
                if (val <= mi) return mi;
                if (val >= ma) return ma;
                return val;
            }
            return new Point(
                impl(source.X, min, max),
                impl(source.Y, min, max));
        }

    }
}
