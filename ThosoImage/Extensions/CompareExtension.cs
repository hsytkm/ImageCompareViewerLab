using System;

namespace ThosoImage.Extensions
{
    public static class CompareExtension
    {
        /// <summary>
        /// 値が指定範囲に収まっているか判定する
        /// </summary>
        /// <typeparam name="T">指定値の型</typeparam>
        /// <param name="val">制限前の値</param>
        /// <param name="fromValue">最小値</param>
        /// <param name="toValue">最大値</param>
        /// <returns>範囲外=false / 範囲内=true</returns>
        public static bool IsRange<T>(this T val, T fromValue, T toValue) where T : IComparable
        {
            if (0 <= val.CompareTo(fromValue))
            {
                if (val.CompareTo(toValue) <= 0) return true;
            }
            return false;
        }

        /// <summary>
        /// 値を指定範囲に収めて返す
        /// </summary>
        /// <typeparam name="T">指定値の型</typeparam>
        /// <param name="val">制限前の値</param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// <returns>制限後の値</returns>
        public static T Limit<T>(this T val, T min, T max) where T : IComparable
        {
            if (0 > max.CompareTo(min)) throw new ArgumentException("Inverse(min>max)");
            if (0 > val.CompareTo(min))
            {
                return min;
            }
            else if (val.CompareTo(max) > 0)
            {
                return max;
            }
            return val;
        }

    }
}
