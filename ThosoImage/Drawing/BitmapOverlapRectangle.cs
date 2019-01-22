using System.Drawing;

namespace ThosoImage.Drawing
{
    public static class BitmapOverlapRectangle
    {
        /// <summary>
        /// 画像に対して四角形を重畳する
        /// </summary>
        /// <param name="source">重畳元の画像PATH</param>
        /// <param name="x">重畳する図形の開始座標</param>
        /// <param name="y">重畳する図形の開始座標</param>
        /// <param name="width">重畳する図形の幅</param>
        /// <param name="height">重畳する図形の高さ</param>
        /// <param name="red">重畳する図形の色指定</param>
        /// <param name="green">重畳する図形の色指定</param>
        /// <param name="blue">重畳する図形の色指定</param>
        /// <param name="thickness">重畳する図形の太さ</param>
        /// <returns>重畳後の画像</returns>
        public static Bitmap GetRectangleOverlapBitmap(this Bitmap source,
            int x, int y, int width, int height,
            int red, int green, int blue, double thickness)
        {
            var rect = new Rectangle(x, y, width, height);
            var color = Color.FromArgb(red, green, blue);   //alpha = 255
            return source.GetRectangleOverlapBitmap(rect, color, thickness);
        }

        /// <summary>
        /// 画像に対して四角形を重畳する
        /// </summary>
        /// <param name="source">重畳元の画像</param>
        /// <param name="rect">重畳する図形</param>
        /// <param name="color">重畳する図形の色</param>
        /// <param name="thickness">重畳する図形の太さ</param>
        public static Bitmap GetRectangleOverlapBitmap(this Bitmap source, Rectangle rect, Color color, double thickness)
        {
            var canvas = new Bitmap(source.Width, source.Height);

            using (var g = Graphics.FromImage(canvas))
            using (var p = new Pen(color, (float)thickness))
            {
                g.DrawImage(source, 0, 0, source.Width, source.Height);
                g.DrawRectangle(p, rect);
            }
            return canvas;
        }

    }
}
