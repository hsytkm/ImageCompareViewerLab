using CursorPixelRender.Models;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace CursorPixelRender.ViewModels
{
    class PixelRenderViewModel : BindableBase
    {
        private readonly ImagePixelReader _imagePixelReader = new ImagePixelReader();

        public ReadOnlyReactiveProperty<int> CursorX { get; }
        public ReadOnlyReactiveProperty<int> CursorY { get; }

        public ReadOnlyReactiveProperty<PixelDataVM> PixelR { get; }
        public ReadOnlyReactiveProperty<PixelDataVM> PixelG { get; }
        public ReadOnlyReactiveProperty<PixelDataVM> PixelGr { get; }
        public ReadOnlyReactiveProperty<PixelDataVM> PixelGb { get; }
        public ReadOnlyReactiveProperty<PixelDataVM> PixelB { get; }

        public ReadOnlyReactiveProperty<PixelDataVM> PixelY { get; }

        public ReadOnlyReactiveProperty<PixelDataVM> PixelL { get; }
        public ReadOnlyReactiveProperty<PixelDataVM> Pixela { get; }
        public ReadOnlyReactiveProperty<PixelDataVM> Pixelb { get; }

        public PixelRenderViewModel()
        {
            var readPixels = _imagePixelReader
                .ObserveProperty(x => x.ReadPixels)
                .ToReadOnlyReactiveProperty(mode: ReactivePropertyMode.DistinctUntilChanged);

            CursorX = readPixels
                .Select(x => x.ReadArea.X)
                .ToReadOnlyReactiveProperty();

            CursorY = readPixels
                .Select(x => x.ReadArea.Y)
                .ToReadOnlyReactiveProperty();

            PixelR = readPixels
                .Select(x => new PixelDataVM(x, PixelColor.R))
                .ToReadOnlyReactiveProperty();

            PixelG = readPixels
                .Select(x => new PixelDataVM(x, PixelColor.G))
                .ToReadOnlyReactiveProperty();

            PixelGr = readPixels
                .Select(x => new PixelDataVM(x, PixelColor.Gr))
                .ToReadOnlyReactiveProperty();

            PixelGb = readPixels
                .Select(x => new PixelDataVM(x, PixelColor.Gb))
                .ToReadOnlyReactiveProperty();

            PixelB = readPixels
                .Select(x => new PixelDataVM(x, PixelColor.B))
                .ToReadOnlyReactiveProperty();

            PixelY = readPixels
                .Select(x => new PixelDataVM(x, PixelColor.Y, 1))   // 小数点第一位表示用の+1
                .ToReadOnlyReactiveProperty();

            PixelL = readPixels
                .Select(x => new PixelDataVM(x, PixelColor.L, 1))   // 小数点第一位表示用の+1
                .ToReadOnlyReactiveProperty();

            Pixela = readPixels
                .Select(x => new PixelDataVM(x, PixelColor.a, 1))   // 小数点第一位表示用の+1
                .ToReadOnlyReactiveProperty();

            Pixelb = readPixels
                .Select(x => new PixelDataVM(x, PixelColor.b, 1))   // 小数点第一位表示用の+1
                .ToReadOnlyReactiveProperty();
        }
    }

    readonly struct PixelDataVM
    {
        /// <summary>
        /// 表示フラグ
        /// </summary>
        public bool IsVisible { get; }

        /// <summary>
        /// 最大値から求めた桁数
        /// </summary>
        public int Digit { get; }

        /// <summary>
        /// 画素値
        /// </summary>
        public double Value { get; }

        public PixelDataVM(ReadPixelsData pixels, PixelColor pixelColor, int digitOffset = 0)
        {
            if (pixels is null) throw new ArgumentNullException(nameof(pixels));

            if (pixels.TryToFindPixelColor(pixelColor, out var pixelData))
            {
                IsVisible = true;
                Digit = GetDigit(pixelData.Max) + digitOffset;
                Value = pixelData.Average;
            }
            else
            {
                IsVisible = false;
                Digit = 0;
                Value = 0;
            }
        }

        /// <summary>
        /// bit幅の最大値の桁数を返す
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetDigit(int num)
        {
            // Mathf.Log10(0)はNegativeInfinityを返すため、別途処理する。
            return (num == 0) ? 1 : ((int)Math.Log10(num) + 1);
        }
    }

}
