using Prism.Mvvm;
using System;
using System.Collections.Generic;
using ThosoImage.Drawing;

namespace OxyPlotInspector.Models
{
    class Histogram : BindableBase
    {
        private string ImageSourcePath;

        // ヒストグラムViewの表示状態フラグ
        private bool _IsShowingHistogramView;
        public bool IsShowingHistogramView
        {
            get => _IsShowingHistogramView;
            set => SetProperty(ref _IsShowingHistogramView, value);
        }

        // ライン上のRGB値配列
        private (byte R, byte G, byte B)[] _RgbLevelLine;
        public (byte R, byte G, byte B)[] RgbLevelLine
        {
            get => _RgbLevelLine;
            private set => SetProperty(ref _RgbLevelLine, value);
        }

        public Histogram()
        {
            // ◆本質でないので無理やり現画像PATHを取得
            ImageSourcePath = MainImageSource.ImageSourcePath;
        }

        // 線の端座標通知(引数の単位は割合)
        public void SetLinePointsRatio((double X1, double Y1, double X2, double Y2) ratio)
        {
            var (Width, Height) = ImageSourcePath.GetImageSize();

            double X1 = ratio.X1 * Width;
            double Y1 = ratio.Y1 * Height;
            double X2 = ratio.X2 * Width;
            double Y2 = ratio.Y2 * Height;
            double distance = Math.Sqrt((X1 - X2) * (X1 - X2) + (Y1 - Y2) * (Y1 - Y2));

            Console.WriteLine(distance);

            int distFloor = (int)Math.Floor(distance);
            var rgbs = new (byte R, byte G, byte B)[distFloor];

            var rand = new Random();
            for (int i = 0; i < distFloor; i++)
            {
                // 未実装
                (byte R, byte G, byte B) rgb = ((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));
                rgbs[i] = rgb;
            }
            RgbLevelLine = rgbs;
        }

    }
}
