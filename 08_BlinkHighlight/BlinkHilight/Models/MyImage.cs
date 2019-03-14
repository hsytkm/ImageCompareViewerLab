using Prism.Mvvm;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BlinkHilight.Models
{
    class MyImage : BindableBase
    {
        private BitmapSource _ImageSource;
        public BitmapSource ImageSource
        {
            get => _ImageSource;
            private set => SetProperty(ref _ImageSource, value);
        }

        public MyImage()
        {
            var ImagePath = @"C:\data\Image1.JPG";

            ImageSource = ImagePath.ToBitmapImage();
        }

        /// <summary>
        /// 表示画像の飽和画素を点滅させる
        /// </summary>
        public async Task BlinkHighlightAsync()
        {
            var source = ImageSource;
            var highlight = source.ToHighlighBitmapSource();

            // 差分(飽和画素)がなければ終わり
            if (source == highlight) return;

            int msec = 400;
            for (int i = 0; i < 3; i++)
            {
                var task0 = Task.Run(() => ImageSource = highlight);
                var task1 = Task.Delay(msec);
                await Task.WhenAll(task0, task1);

                var task2 = Task.Run(() => ImageSource = source);
                var task3 = Task.Delay(msec);
                await Task.WhenAll(task2, task3);
            }
        }

    }
}
