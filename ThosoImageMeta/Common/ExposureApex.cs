using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ThosoImage.Meta
{
    public static class ExposureApex
    {
        private static readonly double Log2 = Math.Log(2.0);

        #region F値 → AV値

        /// <summary>
        /// F値 / AV値 テーブル (ジャスト1/3刻みにしたいので計算で求めない)
        /// </summary>
        private static ReadOnlyDictionary<double, double> Fval2AvMap =
            new ReadOnlyDictionary<double, double>(new Dictionary<double, double>()
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
            });

        /// <summary>
        /// F値(string)からAV値(double)を返す
        /// </summary>
        public static double Fval2Av(string sorig)
        {
            string s = sorig.ToUpper().Replace("F", "");
            double.TryParse(s, out double fval);
            return Fval2Av(fval);
        }

        /// <summary>
        /// F値(double)からAV値(double)を返す
        /// </summary>
        public static double Fval2Av(double fval)
        {
            if (!Fval2AvMap.TryGetValue(fval, out double apex))
            {
                apex = Math.Log(fval * fval) / Log2;
            }

            //Console.WriteLine($"{fval:f2} -> {apex:f3}");
            return apex;
        }

        #endregion

        #region シャッタスピード → TV値

        /// <summary>
        /// シャッタスピード / TV値 テーブル (ジャスト1/3刻みにしたいので計算で求めない)
        /// 1秒より長秒側
        /// </summary>
        private static ReadOnlyDictionary<double, double> Ss2TvMap =
            new ReadOnlyDictionary<double, double>(new Dictionary<double, double>()
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
            });

        /// <summary>
        /// シャッタスピード / TV値 テーブル (ジャスト1/3刻みにしたいので計算で求めない)
        /// 1秒より高速側
        /// </summary>
        private static ReadOnlyDictionary<double, double> InverseSs2TvMap =
            new ReadOnlyDictionary<double, double>(new Dictionary<double, double>()
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
            });

        /// <summary>
        /// シャッタスピード(string)からTV値(double)を返す
        /// </summary>
        public static double Sspeed2Tv(string sorig)
        {
            string s = sorig.ToUpper().Replace("S", "");
            double apex;

            if (s.Contains("1/"))
            {
                double.TryParse(s.Replace("1/", ""), out double ssInv);
                if (!InverseSs2TvMap.TryGetValue(ssInv, out apex))
                    apex = Math.Log(ssInv) / Log2;
            }
            else
            {
                double.TryParse(s, out double ss);
                if (!Ss2TvMap.TryGetValue(ss, out apex))
                    apex = Math.Log(1.0 / ss) / Log2;
            }
            //Console.WriteLine($"{sorig:f2} -> {apex:f3}");

            return apex;
        }

        #endregion

        #region ISO感度 → SV値

        /// <summary>
        /// ISO感度 / SV値 テーブル (ジャスト1/3刻みにしたいので計算で求めない)
        /// </summary>
        private static ReadOnlyDictionary<int, double> Iso2SvMap =
            new ReadOnlyDictionary<int, double>(new Dictionary<int, double>()
            {
                {     50, -1 },
                {     64, -1 + 1 / 3.0 },
                {     80, -1 + 2 / 3.0 },
                {    100, 0 },
                {    125, 0 + 1 / 3.0 },
                {    160, 0 + 2 / 3.0 },
                {    200, 1 },
                {    250, 1 + 1 / 3.0 },
                {    320, 1 + 2 / 3.0 },
                {    400, 2 },
                {    500, 2 + 1 / 3.0 },
                {    640, 2 + 2 / 3.0 },
                {    800, 3 },
                {   1000, 3 + 1 / 3.0 },
                {   1250, 3 + 2 / 3.0 },
                {   1600, 4 },
                {   2000, 4 + 1 / 3.0 },
                {   2500, 4 + 2 / 3.0 },
                {   3200, 5 },
                {   4000, 5 + 1 / 3.0 },
                {   5000, 5 + 2 / 3.0 },
                {   6400, 6 },
                {   8000, 6 + 1 / 3.0 },
                {  10000, 6 + 2 / 3.0 },
                {  12800, 7 },
                {  16000, 7 + 1 / 3.0 },
                {  20000, 7 + 2 / 3.0 },
                {  25600, 8 },
                {  32000, 8 + 1 / 3.0 },
                {  40000, 8 + 2 / 3.0 },
                {  51200, 9 },
                {  64000, 9 + 1 / 3.0 },
                {  80000, 9 + 2 / 3.0 },
                { 102400, 10 },
                { 128000, 10 + 1 / 3.0 },
                { 160000, 10 + 2 / 3.0 },
                { 204800, 11 },
                {  12500, 7 },	// for Leica
                {  25000, 8 },
                {  50000, 9 },
                { 100000, 10 },
                { 200000, 11 },
            });

        /// <summary>
        /// ISO感度(string)からSV値(double)を返す
        /// </summary>
        public static double Iso2Sv(string sorig)
        {
            string s = sorig.ToUpper().Replace("ISO", "");
            int.TryParse(s, out int iso);
            return Iso2Sv(iso);
        }

        /// <summary>
        /// ISO感度(double)からSV値(double)を返す
        /// </summary>
        public static double Iso2Sv(int iso)
        {
            if (!Iso2SvMap.TryGetValue(iso, out double apex))
            {
                apex = Math.Log(iso / 100.0) / Log2;
            }

            //Console.WriteLine($"{iso} -> {apex:f3}");
            return apex;
        }

        #endregion

    }
}
