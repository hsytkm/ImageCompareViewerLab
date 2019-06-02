using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ZoomThumb.Views.Common;

namespace ZoomThumb.Views
{
    class ScrollImageViewer : ScrollViewer
    {
        // 内部コントロールの参照
        public ScrollContentPresenter InsideScrollPresenter { get; private set; }
        public Image InsideImage { get; private set; }

        // ズーム倍率
        private ImageZoomMagnification _ImageZoomMagnification = ImageZoomMagnification.Entire;
        public ImageZoomMagnification ImageZoomMagnification
        {
            get => _ImageZoomMagnification;
            set
            {
                if (!_ImageZoomMagnification.Equals(value))
                {
                    _ImageZoomMagnification = value;
                    UpdateImageZoom();
                }
            }
        }

        // 元画像のサイズ
        private Size _ImageSourcePixelSize;
        public Size ImageSourcePixelSize
        {
            get => _ImageSourcePixelSize;
            set
            {
                if (!_ImageSourcePixelSize.Equals(value))
                {
                    _ImageSourcePixelSize = value;
                    UpdateImageZoom();
                }
            }
        }

        // スクロールバー除いた領域のサイズ（全画面でバーが消えた後にサイズ更新するために必要）
        private Size _ScrollContentActualSize;
        public Size ScrollContentActualSize
        {
            get => _ScrollContentActualSize;
            set
            {
                if (!_ScrollContentActualSize.Equals(value))
                {
                    _ScrollContentActualSize = value;
                    UpdateImageZoom();
                }
            }
        }

        public ScrollImageViewer()
        {
            this.Loaded += (_, __) =>
            {
                InsideScrollPresenter = ViewHelper.GetChildControl<ScrollContentPresenter>(this);

                // 初期サイズはLoadedで取得しようとしたけどイベント来ないのでココで
                //scrollContentPresenter.Loaded += (sender, e) => 
                ScrollContentActualSize = GetControlActualSize(InsideScrollPresenter);
                InsideScrollPresenter.SizeChanged += (sender, e) =>
                {
                    ScrollContentActualSize = e.NewSize; //=ActualSize
                };


                InsideImage = ViewHelper.GetChildControl<Image>(this);
                if (InsideImage is null) throw new NullReferenceException("Not Found Image Control. Invalid xaml.");

                InsideImage.TargetUpdated += (sender, e) =>
                {
                    if (!(e.OriginalSource is Image image)) return;
                    ImageSourcePixelSize = GetImageSourcePixelSize(image);
                };

            };

        }

        private Size GetControlActualSize(FrameworkElement fe) => (fe is null) ? default : new Size(fe.ActualWidth, fe.ActualHeight);

        private Size GetImageSourcePixelSize(Image image)
        {
            if (!(image?.Source is BitmapSource source)) return default;
            return new Size(source.PixelWidth, source.PixelHeight);
        }

        #region ImageZoomMag

        private void UpdateImageZoom()
        {
            var zoomMagnification = ImageZoomMagnification;
            var scrollPresenterSize = GetControlActualSize(InsideScrollPresenter);
            var imageSourceSize = GetImageSourcePixelSize(InsideImage);
            if (imageSourceSize.Width == 0.0 || imageSourceSize.Height == 0.0) return;

            if (!zoomMagnification.IsEntire)
            {
                // ズーム表示に切り替え
                // 画像サイズの更新前にスクロールバーの表示を更新(ContentSizeに影響出るので)
                UpdateScrollBarVisibility(this, zoomMagnification, scrollPresenterSize, imageSourceSize);

                InsideImage.Width = imageSourceSize.Width * zoomMagnification.MagnificationRatio;
                InsideImage.Height = imageSourceSize.Height * zoomMagnification.MagnificationRatio;
            }
            else
            {
                // 全画面表示に切り替え
                // 画像サイズの更新前にスクロールバーの表示を更新(ContentSizeに影響出るので)
                UpdateScrollBarVisibility(this, zoomMagnification, scrollPresenterSize, imageSourceSize);

                var size = GetEntireZoomSize(scrollPresenterSize, imageSourceSize);
                InsideImage.Width = size.Width;
                InsideImage.Height = size.Height;
            }
        }

        private static void UpdateScrollBarVisibility(ScrollViewer scrollViewer, ImageZoomMagnification zoomMag, Size sviewSize, Size sourceSize)
        {
            var visible = ScrollBarVisibility.Hidden;

            // ズームインならスクロールバーを表示
            if (!zoomMag.IsEntire && (GetEntireZoomMagRatio(sviewSize, sourceSize) < zoomMag.MagnificationRatio))
                visible = ScrollBarVisibility.Visible;

            scrollViewer.HorizontalScrollBarVisibility =
            scrollViewer.VerticalScrollBarVisibility = visible;
        }

        #endregion


        #region ZoomSize

        // 全画面表示のサイズを取得
        private static Size GetEntireZoomSize(Size sviewSize, Size sourceSize)
        {
            var imageRatio = sourceSize.Width / sourceSize.Height;

            double width, height;
            if (imageRatio > sviewSize.Width / sviewSize.Height)
            {
                width = sviewSize.Width;      // 横パンパン
                height = sviewSize.Width / imageRatio;
            }
            else
            {
                width = sviewSize.Height * imageRatio;
                height = sviewSize.Height;    // 縦パンパン
            }
            return new Size(width, height);
        }

        // 全画面表示のズーム倍率を取得
        private static double GetEntireZoomMagRatio(Size sviewSize, Size sourceSize) =>
            GetZoomMagRatio(GetEntireZoomSize(sviewSize, sourceSize), sourceSize);

        // 現在のズーム倍率を取得
        private static double GetCurrentZoomMagRatio(Size imageViewSize, Size imageSourceSize) =>
            GetZoomMagRatio(imageViewSize, imageSourceSize);

        // 引数サイズのズーム倍率を求める
        private static double GetZoomMagRatio(Size newSize, Size baseSize) =>
            Math.Min(newSize.Width / baseSize.Width, newSize.Height / baseSize.Height);

        #endregion


    }
}
