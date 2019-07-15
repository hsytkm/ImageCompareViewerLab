using System;
using System.Collections.Generic;

namespace ImageMetaExtractor.Common
{
    public static class ExposureApexSv
    {
        private static readonly double Log2 = Math.Log(2.0);

        /// <summary>
        /// ISO感度 / SV値 テーブル (ジャスト1/3刻みにしたいので計算で求めない)
        /// </summary>
        private static readonly IReadOnlyDictionary<int, double> DictionaryIso2Sv =
            new Dictionary<int, double>()
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
            };

        /// <summary>
        /// ISO感度(string)からSV値(double)を返す
        /// </summary>
        public static double Iso2Sv(string src)
        {
            string s = src.ToUpper().Replace("ISO", "");
            if (int.TryParse(s, out int iso))
                return Iso2Sv(iso);
            return default;
        }

        /// <summary>
        /// ISO感度(double)からSV値(double)を返す
        /// </summary>
        public static double Iso2Sv(int iso)
        {
            if (!DictionaryIso2Sv.TryGetValue(iso, out double apex))
                apex = Math.Log(iso / 100.0) / Log2;

            //Console.WriteLine($"{iso} -> {apex:f3}");
            return apex;
        }

    }
}
