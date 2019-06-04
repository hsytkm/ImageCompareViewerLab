using Prism.Mvvm;
using System;

namespace ZoomThumb.Models
{
    class ViewSettings : BindableBase
    {

        // 各画像の表示エリアを連動させるかフラグ(FALSE=連動しない)
        private bool _IsImageViewerInterlock;
        public bool IsImageViewerInterlock
        {
            get => _IsImageViewerInterlock;
            set => SetProperty(ref _IsImageViewerInterlock, value);
        }

        // 縮小画像の表示可能フラグ(FALSE=表示禁止)
        private bool _CanVisibleReducedImage;
        public bool CanVisibleReducedImage
        {
            get => _CanVisibleReducedImage;
            set => SetProperty(ref _CanVisibleReducedImage, value);
        }

        // 画像上のサンプリング枠の表示フラグ(FALSE=表示しない)
        private bool _IsVisibleImageOverlapSamplingFrame;
        public bool IsVisibleImageOverlapSamplingFrame
        {
            get => _IsVisibleImageOverlapSamplingFrame;
            set => SetProperty(ref _IsVisibleImageOverlapSamplingFrame, value);
        }
        

    }
}
