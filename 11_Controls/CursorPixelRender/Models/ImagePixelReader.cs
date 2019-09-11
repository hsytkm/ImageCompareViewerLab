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
        /// 画素読み出し領域
        /// </summary>
        public ReadPixelArea ReadArea
        {
            get => _readArea;
            private set => SetProperty(ref _readArea, value);
        }
        private ReadPixelArea _readArea;

        /// <summary>
        /// 画素値リスト
        /// </summary>
        public ObservableCollection<ReadPixelData> Pixels { get; } = new ObservableCollection<ReadPixelData>();

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

                    ReadArea = area;

                    Pixels.Clear();
                    foreach (var pixel in pixels) { Pixels.Add(pixel); }
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
                new ReadPixelData(PixelColor.R, r, rgbMax),
                new ReadPixelData(PixelColor.G, g, rgbMax),
                new ReadPixelData(PixelColor.B, b, rgbMax),

                new ReadPixelData(PixelColor.Y, y, rgbMax, isInteger: false),
                new ReadPixelData(PixelColor.L, GetRandomValue(bitSize), labMax, isInteger: false),

                // Labのa,bの最大値（表示幅に使用される）がテキトー
                // 正確には -100～100(?) だが、0～100 と思って動作してる
                new ReadPixelData(PixelColor.a, GetRandomValue(bitSize), labMax, isInteger: false),
                new ReadPixelData(PixelColor.b, GetRandomValue(bitSize), labMax, isInteger: false),
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
                new ReadPixelData(color, GetRandomValue(bitSize), max),
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
