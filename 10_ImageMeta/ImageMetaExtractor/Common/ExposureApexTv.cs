using System;
using System.Collections.Generic;

namespace ImageMetaExtractor.Common
{
    public static class ExposureApexTv
    {
        private static readonly double Log2 = Math.Log(2.0);

        /// <summary>
        /// シャッタスピード / TV値 テーブル (ジャスト1/3刻みにしたいので計算で求めない)
        /// 1秒より長秒側
        /// </summary>
        private static readonly IReadOnlyDictionary<double, double> DictionarySs2Tv =
            new Dictionary<double, double>()
            {
                {  60 , -6 },
                {  50 , -6 + 1 / 3.0 },
                {  40 , -6 + 2 / 3.0 },
                {  30 , -5 },
                {  25 , -5 + 1 / 3.0 },
                {  20 , -5 + 2 / 3.0 },
                {  15 , -4 },
                {  12 , -4 + 1 / 3.0 },
                {  10 , -4 + 2 / 3.0 },
                {  8  , -3 },
                {  6  , -3 + 1 / 3.0 },
                {  5  , -3 + 2 / 3.0 },
                {  4  , -2 },
                {  3.2, -2 + 1 / 3.0 },
                {  2.5, -2 + 2 / 3.0 },
                {  2  , -1 },
                {  1.6, -1 + 1 / 3.0 },
                {  1.3, -1 + 2 / 3.0 },
                {  1  , 0 },
            };

        /// <summary>
        /// シャッタスピード / TV値 テーブル (ジャスト1/3刻みにしたいので計算で求めない)
        /// 1秒より高速側
        /// </summary>
        private static readonly IReadOnlyDictionary<double, double> DictionaryInverseSs2Tv =
            new Dictionary<double, double>()
            {
                {  1     , 0 },
                {  1.3   , 0 + 1 / 3.0 },
                {  1.25  , 0 + 1 / 3.0 },  // forSony
                {  1.6   , 0 + 2 / 3.0 },
                {  2     , 1 },
                {  2.5   , 1 + 1 / 3.0 },
                {  3.2   , 1 + 2 / 3.0 },  // forPana
                {  3     , 1 + 2 / 3.0 },  // forOly
                {  4     , 2 },
                {  5     , 2 + 1 / 3.0 },
                {  6     , 2 + 2 / 3.0 },
                {  8     , 3 },
                {  10    , 3 + 1 / 3.0 },
                {  13    , 3 + 2 / 3.0 },
                {  15    , 4 },
                {  20    , 4 + 1 / 3.0 },
                {  25    , 4 + 2 / 3.0 },
                {  30    , 5 },
                {  40    , 5 + 1 / 3.0 },
                {  50    , 5 + 2 / 3.0 },
                {  60    , 6 },
                {  80    , 6 + 1 / 3.0 },
                {  100   , 6 + 2 / 3.0 },
                {  125   , 7 },
                {  160   , 7 + 1 / 3.0 },
                {  200   , 7 + 2 / 3.0 },
                {  250   , 8 },
                {  320   , 8 + 1 / 3.0 },
                {  400   , 8 + 2 / 3.0 },
                {  500   , 9 },
                {  640   , 9 + 1 / 3.0 },
                {  800   , 9 + 2 / 3.0 },
                {  1000  , 10 },
                {  1250  , 10 + 1 / 3.0 },  // forOly
                {  1300  , 10 + 1 / 3.0 },  // forPana
                {  1600  , 10 + 2 / 3.0 },
                {  2000  , 11 },
                {  2500  , 11 + 1 / 3.0 },
                {  3200  , 11 + 2 / 3.0 },
                {  4000  , 12 },
                {  5000  , 12 + 1 / 3.0 },
                {  6400  , 12 + 2 / 3.0 },
                {  8000  , 13 },
                {  10000 , 13 + 1 / 3.0 },
                {  13000 , 13 + 2 / 3.0 },
                {  12800 , 14 },
                {  16000 , 14 + 1 / 3.0 },
                {  20000 , 14 + 2 / 3.0 },
                {  25000 , 15 },
                {  32000 , 15 + 1 / 3.0 },
                {  40000 , 15 + 2 / 3.0 },
                {  50000 , 16 },
                {  64000 , 16 + 1 / 3.0 },
            };

        /// <summary>
        /// シャッタスピード(string)からTV値(double)を返す
        /// </summary>
        public static double Sspeed2Tv(string src)
        {
            string s = src.ToUpper().Replace("S", "");
            double apex = default;

            if (s.Contains("1/"))
            {
                if (double.TryParse(s.Replace("1/", ""), out double ssInv))
                {
                    if (!DictionaryInverseSs2Tv.TryGetValue(ssInv, out apex))
                        apex = Math.Log(ssInv) / Log2;
                }
            }
            else
            {
                if (double.TryParse(s, out double ss))
                {
                    if (!DictionarySs2Tv.TryGetValue(ss, out apex))
                        apex = Math.Log(1.0 / ss) / Log2;
                }
            }

            //Console.WriteLine($"{sorig:f2} -> {apex:f3}");
            return apex;
        }

    }
}
