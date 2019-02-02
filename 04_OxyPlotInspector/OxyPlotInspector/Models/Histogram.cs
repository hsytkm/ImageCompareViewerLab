using Prism.Mvvm;
using ThosoImage.Pixels;

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
            RgbLevelLine = ImageSourcePath.GetRgbLevels(ratio.X1, ratio.Y1, ratio.X2, ratio.Y2);
        }

    }
}
