using CursorPixelRender.Models;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Linq;
using System.Reactive.Linq;

namespace CursorPixelRender.ViewModels
{
    class PixelRenderViewModel : BindableBase
    {
        private readonly ImagePixelReader _imagePixelReader = new ImagePixelReader();
        //private ReadOnlyReactiveProperty<ReadPixelsData> _readPixels;

        public ReadOnlyReactiveProperty<bool> IsVisibleR { get; }
        public ReadOnlyReactiveProperty<bool> IsVisibleG { get; }
        public ReadOnlyReactiveProperty<bool> IsVisibleB { get; }
        public ReadOnlyReactiveProperty<bool> IsVisibleGr { get; }
        public ReadOnlyReactiveProperty<bool> IsVisibleGb { get; }

        public ReadOnlyReactiveProperty<double> AveR { get; }
        public ReadOnlyReactiveProperty<double> AveG { get; }
        public ReadOnlyReactiveProperty<double> AveB { get; }
        public ReadOnlyReactiveProperty<double> AveGr { get; }
        public ReadOnlyReactiveProperty<double> AveGb { get; }

        public PixelRenderViewModel()
        {
#if false
            var readPixels = _imagePixelReader
                .ObserveProperty(x => x.ReadPixels)
                .ToReadOnlyReactiveProperty();

            AveR = readPixels.Select(x => x.AveR).ToReadOnlyReactiveProperty();
            AveG = readPixels.Select(x => x.AveG).ToReadOnlyReactiveProperty();
            AveB = readPixels.Select(x => x.AveB).ToReadOnlyReactiveProperty();
#endif

#if true
            var readPixels = _imagePixelReader
                .ObserveProperty(x => x.ReadPixels2)
                .ToReadOnlyReactiveProperty(mode: ReactivePropertyMode.DistinctUntilChanged);

            IsVisibleR = readPixels
                .Select(x => x.IsContainPixelColor(PixelColor.R))
                .ToReadOnlyReactiveProperty();
            AveR = readPixels
                .Select(x => x.GetPixelAverage(PixelColor.R))
                .ToReadOnlyReactiveProperty();

            IsVisibleG = readPixels
                .Select(x => x.IsContainPixelColor(PixelColor.G))
                .ToReadOnlyReactiveProperty();
            AveG = readPixels
                .Select(x => x.GetPixelAverage(PixelColor.G))
                .ToReadOnlyReactiveProperty();

            IsVisibleB = readPixels
                .Select(x => x.IsContainPixelColor(PixelColor.B))
                .ToReadOnlyReactiveProperty();
            AveB = readPixels
                .Select(x => x.GetPixelAverage(PixelColor.B))
                .ToReadOnlyReactiveProperty();

            IsVisibleGr = readPixels
                .Select(x => x.IsContainPixelColor(PixelColor.Gr))
                .ToReadOnlyReactiveProperty();
            AveGr = readPixels
                .Select(x => x.GetPixelAverage(PixelColor.Gr))
                .ToReadOnlyReactiveProperty();

            IsVisibleGb = readPixels
                .Select(x => x.IsContainPixelColor(PixelColor.Gb))
                .ToReadOnlyReactiveProperty();
            AveGb = readPixels
                .Select(x => x.GetPixelAverage(PixelColor.Gb))
                .ToReadOnlyReactiveProperty();

#endif
        }

    }
}
