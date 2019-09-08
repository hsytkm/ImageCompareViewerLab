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
        public ReadPixelsData ReadPixels2
        {
            get => _readPixels2;
            private set => SetProperty(ref _readPixels2, value);
        }
        private ReadPixelsData _readPixels2;

        private bool _togglePixelType;

        private readonly Random _random = new Random();

        public ImagePixelReader()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(3000))
                .Subscribe(_ => _togglePixelType = !_togglePixelType);

            Observable.Interval(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ =>
                {
                    var pixels = _togglePixelType ? GetRgbData() : GetRawData();

                    ReadPixels2 = new ReadPixelsData(pixels);

                    //ReadPixels3.Clear();
                    //foreach (var p in pixels)
                    //{
                    //    ReadPixels3.Add(p);
                    //}
                });
        }

        private ReadPixelData[] GetRgbData()
        {
            double get_random() => _random.NextDouble() * 256;
            int max = 255;

            var pixels = new[]
            {
                new ReadPixelData()
                {
                    Color = PixelColor.R,
                    Max = max,
                    Average = get_random(),
                },
                new ReadPixelData()
                {
                    Color = PixelColor.G,
                    Max = max,
                    Average = get_random(),
                },
                new ReadPixelData()
                {
                    Color = PixelColor.B,
                    Max = max,
                    Average = get_random(),
                },
            };

            return pixels;
        }

        private ReadPixelData[] GetRawData()
        {
            double get_random() => _random.NextDouble() * 256;
            int max = 255;

            PixelColor color;
            switch (_random.Next(0, 4))
            {
                case 0: color = PixelColor.R; break;
                case 1: color = PixelColor.Gr; break;
                case 2: color = PixelColor.Gb; break;
                case 3: color = PixelColor.B; break;
                default: throw new Exception();
            }

            return new[]
            {
                new ReadPixelData()
                {
                    Color = color,
                    Max = max,
                    Average = get_random(),
                },
            };
        }

    }
}
