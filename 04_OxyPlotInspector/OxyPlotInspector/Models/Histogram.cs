using Prism.Mvvm;
using System;
using ThosoImage.Drawing;

namespace OxyPlotInspector.Models
{
    class Histogram : BindableBase
    {
        // ヒストグラムViewの表示状態フラグ
        private bool _IsShowingHistgramView;
        public bool IsShowingHistgramView
        {
            get => _IsShowingHistgramView;
            set => SetProperty(ref _IsShowingHistgramView, value);
        }

        public Histogram()
        {
            // ◆本質でないので無理やり現画像サイズを取得
            ImageSize = MainImageSource.ImageSourcePath.GetImageSize();
        }

        private (int Width, int Height) ImageSize;

        // 線の端座標通知(引数の単位は割合)
        public void SetLinePointsRatio((double X1, double Y1, double X2, double Y2) ratio)
        {
            double X1 = ratio.X1 * ImageSize.Width;
            double Y1 = ratio.Y1 * ImageSize.Height;
            double X2 = ratio.X2 * ImageSize.Width;
            double Y2 = ratio.Y2 * ImageSize.Height;
            double distance = Math.Sqrt((X1 - X2) * (X1 - X2) + (Y1 - Y2) * (Y1 - Y2));

            Console.WriteLine(distance);
        }

    }
}
