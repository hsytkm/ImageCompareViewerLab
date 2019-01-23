using System;
using System.Windows;

namespace ThosoImage.Wpf.Extensions
{
    public static class WindowsRectExtension
    {
#if false // どれも使わなさそうなので必要になるまで無効化

        /// <summary>
        /// Rect型のX/Y座標を補正
        /// </summary>
        /// <param name="rect">補正前のRect</param>
        /// <param name="point">補正量</param>
        /// <returns>補正後のRect</returns>
        public static Rect ShiftXY(this Rect rect, Point point)
        {
            return new Rect(
                rect.X + point.X,
                rect.Y + point.Y,
                rect.Width, rect.Height);
        }

        /// <summary>
        /// Rect型のサイズを変更
        /// </summary>
        /// <param name="rect">補正前のRect</param>
        /// <param name="shiftWidth">幅の補正量</param>
        /// <param name="shiftHeight">高さの補正量</param>
        /// <returns>補正後のRect</returns>
        public static Rect ShiftSize(this Rect rect, int shiftWidth, int shiftHeight)
        {
            return new Rect(
                rect.X, rect.Y,
                Math.Max(1, rect.Width + shiftWidth),
                Math.Max(1, rect.Height + shiftHeight));
        }

        /// <summary>
        /// Rectの座標とサイズをSize型で乗算
        /// </summary>
        /// <param name="rect">補正前のRect</param>
        /// <param name="size">乗算値</param>
        /// <returns>補正後のRect</returns>
        public static Rect Multiply(this Rect rect, Size size)
        {
            return new Rect(
                rect.X * size.Width,
                rect.Y * size.Height,
                rect.Width * size.Width,
                rect.Height * size.Height);
        }

        /// <summary>
        /// Rectの座標とサイズをSize型で除算
        /// </summary>
        /// <param name="rect">補正前のRect</param>
        /// <param name="size">除算値</param>
        /// <returns>補正後のRect</returns>
        public static Rect Division(this Rect rect, Size size)
        {
            if (double.IsNaN(size.Width) || size.Width == 0) throw new ArgumentException(nameof(size.Width));
            if (double.IsNaN(size.Height) || size.Height == 0) throw new ArgumentException(nameof(size.Height));

            return new Rect(
                rect.X / size.Width,
                rect.Y / size.Height,
                rect.Width / size.Width,
                rect.Height / size.Height);
        }

        /// <summary>
        /// Rect型の値を制限(サイズ保持を優先して制限)
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>制限後のRect</returns>
        public static Rect Limit(this Rect rect, double min, double max)
        {
            double impl(double val, double mi, double ma)
            {
                if (val <= mi) return mi;
                if (val >= ma) return ma;
                return val;
            }
            double minWidth = impl(rect.Width, min, max);
            double minHeight = impl(rect.Height, min, max);
            return new Rect(
                impl(rect.X, min, max - minWidth),
                impl(rect.Y, min, max - minHeight),
                minWidth, minHeight);
        }

        /// <summary>
        /// Rect型のサイズのみを制限
        /// </summary>
        /// <param name="rect">制限前のRect</param>
        /// <param name="maxSize">サイズの最大値</param>
        /// <returns>制限後のRect</returns>
        public static Rect LimitSize(this Rect rect, Size maxSize)
        {
            return new Rect(
                rect.X, rect.Y,
                Math.Max(rect.Width, maxSize.Width),
                Math.Max(rect.Height, maxSize.Height));
        }

        /// <summary>
        /// Rect型を指定角度で回転
        /// </summary>
        /// <param name="rect">回転前のRect</param>
        /// <param name="angle">回転角度</param>
        /// <param name="baseSize"></param>
        /// <returns>回転後のRect</returns>
        public static Rect Rotate(this Rect rect, int angle, Size baseSize)
        {
            angle %= 360;
            if ((angle % 90) != 0) throw new ArgumentException(nameof(angle));

            if (angle == 0)
            {
                return rect;
            }
            else if (angle == 90)
            {
                return new Rect(
                    baseSize.Height - (rect.Y + rect.Height),
                    rect.X,
                    rect.Height,
                    rect.Width);
            }
            else if (angle == 180)
            {
                return new Rect(
                    baseSize.Width - (rect.X + rect.Width),
                    baseSize.Height - (rect.Y + rect.Height),
                    rect.Width,
                    rect.Height);
            }
            else //if (angle == 270)
            {
                return new Rect(
                    rect.Y,
                    baseSize.Width - (rect.X + rect.Width),
                    rect.Height,
                    rect.Width);
            }
        }
#endif
    }
}
