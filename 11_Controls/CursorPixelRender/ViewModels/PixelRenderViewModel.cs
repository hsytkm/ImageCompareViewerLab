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

        public ReadOnlyReactiveCollection<PixelDataVM> Pixels { get; }

        public PixelRenderViewModel()
        {
            CursorX = _imagePixelReader
                .ObserveProperty(x => x.ReadArea)
                .Select(x => x.X)
                .ToReadOnlyReactiveProperty();

            CursorY = _imagePixelReader
                .ObserveProperty(x => x.ReadArea)
                .Select(x => x.Y)
                .ToReadOnlyReactiveProperty();

            Pixels = _imagePixelReader.Pixels
                .ToReadOnlyReactiveCollection(x => new PixelDataVM(x));
        }
    }

    readonly struct PixelDataVM
    {
        public string Name { get; }

        public int Digit { get; }

        public string Message { get; }

        public PixelDataVM(ReadPixelData pixel)
        {
            var decimalPlace = pixel.IsInteger ? 0 : 1;

            Name = Enum.GetName(typeof(PixelColor), pixel.Color);
            Digit = GetDigit(pixel.Max) + decimalPlace;
            Message = pixel.Average.ToString($"f{decimalPlace}");
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
