using Prism.Mvvm;
using ThosoImage.Pixels;

namespace OxyPlotInspector.Models
{
    class ImageLineLevels : BindableBase
    {
        // ◆本質でないので無理やり現画像PATHを取得
        private readonly string ImageSourcePath = MainImageSource.ImageSourcePath;

        // 線上画素値の読み込みクラス
        private BitmapLinePixelReader LinePixelReader;

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

        // 対象画像の画素値全読み処理
        public void LoadImagePixels() => LinePixelReader = new BitmapLinePixelReader(ImageSourcePath);

        // 線の端座標からRGBレベルを求める(引数の単位は割合0~1)
        public void SetLinePointsRatio((double X1, double Y1, double X2, double Y2) ratio)
        {
            RgbLevelLine = LinePixelReader?.GetRgbLineLevelsRatio(ratio.X1, ratio.Y1, ratio.X2, ratio.Y2);
        }

        // 情報の初期化(+仕様メモリの解放)
        public void ReleaseLinePoints()
        {
            RgbLevelLine = null;    // 線を非表示化
            LinePixelReader = null; // 対象画像の全読み画素値を解放
        }

    }
}
