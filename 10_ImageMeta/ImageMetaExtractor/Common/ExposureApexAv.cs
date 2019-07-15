using System;
using System.Collections.Generic;

namespace ImageMetaExtractor.Common
{
    public static class ExposureApexAv
    {
        private static readonly double Log2 = Math.Log(2.0);

        /// <summary>
        /// F値 / AV値 テーブル (ジャスト1/3刻みにしたいので計算で求めない)
        /// </summary>
        private static readonly IReadOnlyDictionary<double, double> DictionaryFval2Av =
            new Dictionary<double, double>()
            {
                {  1.0, 0 },
                {  1.1, 0 + 1 / 3.0 },
                {  1.2, 0 + 2 / 3.0 },
                {  1.4, 1 },
                {  1.6, 1 + 1 / 3.0 },
                {  1.8, 1 + 2 / 3.0 },
                {  2.0, 2 },
                {  2.2, 2 + 1 / 3.0 },
                {  2.5, 2 + 2 / 3.0 },
                {  2.8, 3 },
                {  3.2, 3 + 1 / 3.0 },
                {  3.5, 3 + 2 / 3.0 },
                {  4.0, 4 },
                {  4.5, 4 + 1 / 3.0 },
                {  5.0, 4 + 2 / 3.0 },
                {  5.6, 5 },
                {  6.3, 5 + 1 / 3.0 },
                {  7.1, 5 + 2 / 3.0 },
                {  8.0, 6 },
                {  9.0, 6 + 1 / 3.0 },
                { 10.0, 6 + 2 / 3.0 },
                { 11.0, 7 },
                { 13.0, 7 + 1 / 3.0 },
                { 14.0, 7 + 2 / 3.0 },
                { 16.0, 8 },
                { 18.0, 8 + 1 / 3.0 },
                { 20.0, 8 + 2 / 3.0 },
                { 22.0, 9 },
                { 25.0, 9 + 1 / 3.0 },
                { 28.0, 9 + 2 / 3.0 },
                { 29.0, 9 + 2 / 3.0 },    // Sony
                { 32.0, 10 },
            };

        /// <summary>
        /// F値(string)からAV値(double)を返す
        /// </summary>
        public static double Fval2Av(string src)
        {
            string s = src.ToUpper().Replace("F", "");
            if (double.TryParse(s, out double fval))
                return Fval2Av(fval);
            return default;
        }

        /// <summary>
        /// F値(double)からAV値(double)を返す
        /// </summary>
        public static double Fval2Av(double fval)
        {
            if (!DictionaryFval2Av.TryGetValue(fval, out double apex))
                apex = Math.Log(fval * fval) / Log2;

            //Console.WriteLine($"{fval:f2} -> {apex:f3}");
            return apex;
        }

    }
}
