using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ThosoImage.Wpf.Drawing
{
    public static class BitmapConverterExtension
    {
        /// <summary>
        /// BitmapをBitmapImageに変換
        /// </summary>
        /// <param name="bitmap">Bitmap画像</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            if (bitmap is null) throw new ArgumentNullException();

            var image = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
            }
            return image;
        }

        /// <summary>
        /// BitmapをBitmapSourceに変換
        /// </summary>
        /// <param name="bitmap">Bitmap画像</param>
        /// <returns>BitmapSource</returns>
        public static BitmapSource ToBitmapSource1(this Bitmap bitmap)
        {
            if (bitmap is null) throw new ArgumentNullException();

            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);

                // BitmapFrameはBitmapSourceを継承しているのでそのまま渡せばOK
                var frame = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                frame.Freeze();

                return (BitmapSource)frame;
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// BitmapをBitmapSourceに変換
        /// https://qiita.com/YSRKEN/items/a24bf2173f0129a5825c
        /// </summary>
        /// <param name="bitmap">Bitmap画像</param>
        /// <returns>BitmapSource</returns>
        public static BitmapSource ToBitmapSource2(this Bitmap bitmap)
        {
            if (bitmap is null) throw new ArgumentNullException();

            var hBitmap = bitmap.GetHbitmap();

            try
            {
                // HBitmapからBitmapSourceを作成
                var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
                );
                bitmapSource.Freeze();
                return bitmapSource;
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        /// <summary>
        /// BitmapSourceをBitmapに変換
        /// https://qiita.com/YSRKEN/items/a24bf2173f0129a5825c
        /// </summary>
        /// <param name="bitmapSource">BitmapSource</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this BitmapSource bitmapSource)
        {
            var bitmap = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, PixelFormat.Format32bppPArgb);

            var bitmapData = bitmap.LockBits(
                new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);

            bitmapSource.CopyPixels(Int32Rect.Empty, bitmapData.Scan0,
                bitmapData.Height * bitmapData.Stride, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }


    }
}
