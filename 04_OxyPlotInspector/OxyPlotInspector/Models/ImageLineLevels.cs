using Prism.Mvvm;
using ThosoImage.Pixels;

namespace OxyPlotInspector.Models
{
    class ImageLineLevels : BindableBase
    {
        // ◆本質でないので無理やり現画像PATHを取得
        private string ImageSourcePath = MainImageSource.ImageSourcePath;

        // Viewの表示状態フラグ
        private bool _IsShowingView;
        public bool IsShowingView
        {
            get => _IsShowingView;
            set => SetProperty(ref _IsShowingView, value);
        }

        // ライン上のRGB値配列
        private (byte R, byte G, byte B)[] _RgbLevelLine;
        public (byte R, byte G, byte B)[] RgbLevelLine
        {
            get => _RgbLevelLine;
            private set => SetProperty(ref _RgbLevelLine, value);
        }

        public ImageLineLevels() { }

        // 線の端座標からRGBレベルを求める(引数の単位は割合0~1)
        public void SetLinePointsRatio((double X1, double Y1, double X2, double Y2) ratio)
        {
            RgbLevelLine = ImageSourcePath.GetRgbLineLevels(ratio.X1, ratio.Y1, ratio.X2, ratio.Y2);
        }

        public void ClearLinePoints() => RgbLevelLine = null;

    }
}
