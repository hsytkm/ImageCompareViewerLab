using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlotInspector.Models;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace OxyPlotInspector.ViewModels
{
    class ImageHistogramViewModel : BindableBase
    {
        private PlotModel _OxyHistogram;
        public PlotModel OxyHistogram
        {
            get => _OxyHistogram;
            private set => SetProperty(ref _OxyHistogram, value);
        }

        private readonly LineSeries blueLine;
        private readonly LineSeries greenLine;
        private readonly LineSeries redLine;

        public ImageHistogramViewModel()
        {
            var pm = new PlotModel { Title = "RGB histogram" };

            redLine = new LineSeries { Color = OxyColors.Red };
            greenLine = new LineSeries { Color = OxyColors.Green };
            blueLine = new LineSeries { Color = OxyColors.Blue };

            redLine.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
            greenLine.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
            blueLine.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;

            pm.Series.Add(redLine);
            pm.Series.Add(greenLine);
            pm.Series.Add(blueLine);

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

            OxyHistogram = pm;

            // とりあえず乱数を設定
            var r = new Random();
            for (int i = 0; i < 100; i++)
            {
                this.redLine.Points.Add(new DataPoint(i, r.Next(255)));
                this.greenLine.Points.Add(new DataPoint(i, r.Next(255)));
                this.blueLine.Points.Add(new DataPoint(i, r.Next(255)));
            }
        }


    }
}
