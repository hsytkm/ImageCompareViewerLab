using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace CursorPixelRender.Models
{
    class ImagePixelReader : BindableBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ReadPixelsData ReadPixels
        {
            get => _readPixels;
            private set => SetProperty(ref _readPixels, value);
        }
        private ReadPixelsData _readPixels;

        private bool _togglePixelType;

        private readonly Random _random = new Random();

        public ImagePixelReader()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(3000))
                .Subscribe(_ => _togglePixelType = !_togglePixelType);

            Observable.Interval(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ =>
                {
                    var area = GetRandomReadPixelArea();
                    var pixels = _togglePixelType ? GetRgbData() : GetRawData();

                    ReadPixels = new ReadPixelsData(area, pixels);
                });
        }

        private ReadPixelData[] GetRgbData()
        {
            int bitSize = 8;
            int rgbMax = (1 << bitSize) - 1;
            int labMax = 100;

            var r = GetRandomValue(bitSize);
            var g = GetRandomValue(bitSize);
            var b = GetRandomValue(bitSize);
            var y = CalcY(r, g, b);

            return new[]
            {
                new ReadPixelData(PixelColor.R, rgbMax, r),
                new ReadPixelData(PixelColor.G, rgbMax, g),
                new ReadPixelData(PixelColor.B, rgbMax, b),
                new ReadPixelData(PixelColor.Y, rgbMax, y),
                new ReadPixelData(PixelColor.L, labMax, GetRandomValue(bitSize)),

                // Labのa,bの最大値（表示幅に使用される）がテキトー
                // 正確には -100～100(?) だが、0～100 と思って動作してる
                new ReadPixelData(PixelColor.a, labMax, GetRandomValue(bitSize)),
                new ReadPixelData(PixelColor.b, labMax, GetRandomValue(bitSize)),
            };
        }

        private ReadPixelData[] GetRawData()
        {
            int bitSize = 14;
            int max = (1 << bitSize) - 1;

            PixelColor color;
            switch (_random.Next(0, 4))
            {
                case 0: color = PixelColor.R; break;
                case 1: color = PixelColor.Gr; break;
                case 2: color = PixelColor.Gb; break;
                case 3: color = PixelColor.B; break;
                default: throw new NotSupportedException();
            }

            return new[]
            {
                new ReadPixelData(color, max, GetRandomValue(bitSize)),
            };
        }

        private double GetRandomValue(int bitSize) =>
            _random.NextDouble() * (1 << bitSize);

        private ReadPixelArea GetRandomReadPixelArea() =>
            new ReadPixelArea(_random.Next(0, 6000), _random.Next(0, 4000), 1, 1);

        private static double CalcY(double r, double g, double b) =>
            0.299 * r + 0.587 * g + 0.114 * b;

    }
}
