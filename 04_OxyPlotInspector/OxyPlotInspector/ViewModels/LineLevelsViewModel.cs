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
    class LineLevelsViewModel : BindableBase
    {
        private readonly ImageLineLevels LineLevels = ModelMaster.Instance.LineLevels;

        public ReadOnlyReactiveProperty<PlotModel> OxyLineLevels { get; }
        public ReactiveProperty<bool> IsVisibleRch { get; } = new ReactiveProperty<bool>(initialValue: true);
        public ReactiveProperty<bool> IsVisibleGch { get; } = new ReactiveProperty<bool>(initialValue: true);
        public ReactiveProperty<bool> IsVisibleBch { get; } = new ReactiveProperty<bool>(initialValue: true);

        public LineLevelsViewModel()
        {
            // 画素値の読み出し要求(Viewの表示中はずっと読み込んでおく)
            LineLevels.LoadImagePixels();

            OxyLineLevels = LineLevels
                .ObserveProperty(x => x.RgbLevelLine)
                .Select(x => GetPlotModelSkelton(x))
                .ToReadOnlyReactiveProperty();

            IsVisibleRch
                .CombineLatest(OxyLineLevels, (isVisible, line) => (isVisible, line))
                .Subscribe(x => UpdateSeriesVisible(x.line, x.isVisible, 1));

            IsVisibleGch
                .CombineLatest(OxyLineLevels, (isVisible, line) => (isVisible, line))
                .Subscribe(x => UpdateSeriesVisible(x.line, x.isVisible, 2));

            IsVisibleBch
                .CombineLatest(OxyLineLevels, (isVisible, line) => (isVisible, line))
                .Subscribe(x => UpdateSeriesVisible(x.line, x.isVisible, 0));
        }

        private PlotModel GetPlotModelSkelton(ReadOnlySpan<(byte R, byte G, byte B)> rgbs)
        {
            var pm = new PlotModel();
            //pm.Title = "Pixel Level";

            var rLine = new LineSeries { Color = OxyColors.Red, StrokeThickness = 1.0 };
            var gLine = new LineSeries { Color = OxyColors.Green, StrokeThickness = 1.0 };
            var bLine = new LineSeries { Color = OxyColors.Blue, StrokeThickness = 1.0 };

            if (rgbs != null)
            {
                for (int i = 0; i < rgbs.Length; i++)
                {
                    rLine.Points.Add(new DataPoint(i, rgbs[i].R));
                    gLine.Points.Add(new DataPoint(i, rgbs[i].G));
                    bLine.Points.Add(new DataPoint(i, rgbs[i].B));
                }
            }

            // B->R->Gの順で描画(Gを全面に)
            pm.Series.Add(bLine);
            pm.Series.Add(rLine);
            pm.Series.Add(gLine);

            pm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 256,
                MajorStep = 64,
                MinorStep = 16,
                MinimumMajorStep = 1,
                MinimumMinorStep = 1,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = "Level@8bit"
            });

            pm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                //Maximum Auto
                //MajorStep Auto
                //MinorStep Auto
                MinimumMajorStep = 1,
                MinimumMinorStep = 1,
                Title = "Length"
            });

            return pm;
        }

        // 線の表示を切り替える
        private static void UpdateSeriesVisible(PlotModel pm, bool isVisible, int index)
        {
            if (pm is null) return;
            if (index < pm.Series.Count)
            {
                pm.Series[index].IsVisible = isVisible;
                pm.InvalidatePlot(true);   // View更新
            }
        }


    }
}
