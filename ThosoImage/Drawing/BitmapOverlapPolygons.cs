using System;
using System.Collections.Generic;
using System.Drawing;

namespace ThosoImage.Drawing
{
    public static class BitmapOverlapPolygons
    {
        /// <summary>
        /// 画像に対して多角形を重畳する
        /// </summary>
        /// <param name="source">重畳元の画像PATH</param>
        /// <param name="pointsList">重畳する図形リスト</param>
        /// <param name="red">重畳する図形の色指定</param>
        /// <param name="green">重畳する図形の色指定</param>
        /// <param name="blue">重畳する図形の色指定</param>
        /// <param name="thickness">重畳する図形の太さ</param>
        /// <returns>重畳後の画像</returns>
        public static Bitmap GetPolygonOverlapBitmap(this Bitmap source,
            IReadOnlyList<Point[]> pointsList, int red, int green, int blue, double thickness)
        {
            var color = Color.FromArgb(red, green, blue);   //alpha = 255
            return source.GetPolygonOverlapBitmap(pointsList, color, thickness);
        }

        /// <summary>
        /// 画像に対して多角形を重畳する
        /// </summary>
        /// <param name="source">重畳元の画像</param>
        /// <param name="pointsList">重畳する図形リスト</param>
        /// <param name="color">重畳する図形の色</param>
        /// <param name="thickness">重畳する図形の太さ</param>
        public static Bitmap GetPolygonOverlapBitmap(this Bitmap source,
            IReadOnlyList<Point[]> pointsList, Color color, double thickness)
        {
            if (source is null) throw new ArgumentNullException();
            if (pointsList is null) throw new ArgumentNullException();

            var canvas = new Bitmap(source.Width, source.Height);

            using (var g = Graphics.FromImage(canvas))
            using (var p = new Pen(color, (float)thickness))
            {
                g.DrawImage(source, 0, 0, source.Width, source.Height);
                foreach (var points in pointsList)
                {
                    g.DrawPolygon(p, points);
                }
            }
            return canvas;
        }

    }
}
