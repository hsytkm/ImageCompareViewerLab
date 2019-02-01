using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlotInspector.Models;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Linq;

namespace OxyPlotInspector.ViewModels
{
    class ImageHistogramViewModel : BindableBase
    {
        private readonly Histogram Histogram = ModelMaster.Instance.Histogram;

        private ReactiveProperty<PlotModel> _OxyHistogram;
        public ReactiveProperty<PlotModel> OxyHistogram
        {
            get => _OxyHistogram;
            private set => SetProperty(ref _OxyHistogram, value);
        }

        public ImageHistogramViewModel()
        {
            OxyHistogram = Histogram
                .ObserveProperty(x => x.RgbLevelLine)
                .Where(x => x != null)
                .Throttle(TimeSpan.FromMilliseconds(100)) // 指定期間に値が通過しなかったら最後の一つを流す
                .Select(x => GetPlotModelSkelton(x))
                .ToReactiveProperty();
        }

        private PlotModel GetPlotModelSkelton((byte R, byte G, byte B)[] rgb)
        {
            var pm = new PlotModel { Title = "RGB histogram" };
            var rLine = new LineSeries { Color = OxyColors.Red };
            var gLine = new LineSeries { Color = OxyColors.Green };
            var bLine = new LineSeries { Color = OxyColors.Blue };

            for (int i = 0; i < rgb.Length; i++)
            {
                rLine.Points.Add(new DataPoint(i, rgb[i].R));
                gLine.Points.Add(new DataPoint(i, rgb[i].G));
                bLine.Points.Add(new DataPoint(i, rgb[i].B));
            }

            pm.Series.Add(rLine);
            pm.Series.Add(gLine);
            pm.Series.Add(bLine);

            pm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 256,
                MajorStep = 64,
                MinorStep = 16,
                Title = "Level@8bit"
            });

            pm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                //Maximum = 1000,
                Title = "Pixel Length"
            });

            return pm;
        }
        
    }
}
