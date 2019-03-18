using System;

namespace ZoomThumb.ViewModels
{
    /// <summary>
    /// 画像の倍率管理
    /// </summary>
    public struct ImageZoomMagnification
    {
        private static readonly double MagRatioMin = Math.Pow(2, -5);   // 3.1%
        private static readonly double MagRatioMax = Math.Pow(2, 5);    // 3200%
        private static readonly double MagStep = 2.0;                   // 2倍

        public bool IsEntire { get; private set; }
        public double MagnificationRatio { get; private set; }

        private ImageZoomMagnification(bool flag)
        {
            if (!flag) throw new ArgumentException(nameof(flag));
            IsEntire = true;
            MagnificationRatio = double.NaN;
        }

        private ImageZoomMagnification(double mag)
        {
            IsEntire = false;
            MagnificationRatio = mag;
        }

        public static ImageZoomMagnification Entire = new ImageZoomMagnification(true);
        private static ImageZoomMagnification MagX1 = new ImageZoomMagnification(1.0);

        public ImageZoomMagnification MagnificationToggle() => IsEntire ? MagX1 : Entire;

        private ImageZoomMagnification ZoomMagnification(double currentMag, double ratio)
        {
            // ホイールすると2の冪乗になるよう元の倍率を補正する
            double currentMagPowerRaw = Math.Log(currentMag) / Math.Log(2);
            double currentMagRound = Math.Pow(2, Math.Round(currentMagPowerRaw));

            double newMag = currentMagRound * ratio;
            if (newMag < MagRatioMin) newMag = MagRatioMin;
            else if (newMag > MagRatioMax) newMag = MagRatioMax;
            return new ImageZoomMagnification(newMag);
        }

        public ImageZoomMagnification ZoomMagnification(double currentMag, bool isZoomIn)
        {
            var step = isZoomIn ? MagStep : 1.0 / MagStep;
            return ZoomMagnification(currentMag, step);
        }

    }
}
