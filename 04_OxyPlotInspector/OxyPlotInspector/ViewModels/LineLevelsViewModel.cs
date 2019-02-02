using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlotInspector.Models;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;

namespace OxyPlotInspector.ViewModels
{
    class LineLevelsViewModel : BindableBase
    {
        private readonly ImageLineLevels LineLevels = ModelMaster.Instance.LineLevels;

        #region RGB Channels

        private bool _IsVisibleRch = true;
        public bool IsVisibleRch
        {
            get => _IsVisibleRch;
            set
            {
                if (SetProperty(ref _IsVisibleRch, value))
                    UpdateSeriesVisible(0, value);
            }
        }

        private bool _IsVisibleGch = true;
        public bool IsVisibleGch
        {
            get => _IsVisibleGch;
            set
            {
                if (SetProperty(ref _IsVisibleGch, value))
                    UpdateSeriesVisible(1, value);
            }
        }

        private bool _IsVisibleBch = true;
        public bool IsVisibleBch
        {
            get => _IsVisibleBch;
            set
            {
                if (SetProperty(ref _IsVisibleBch, value))
                    UpdateSeriesVisible(2, value);
            }
        }

        // 線の表示を切り替える
        private void UpdateSeriesVisible(int index, bool isVisible)
        {
            var pm = OxyLineLevels.Value;
            int seriesCount = pm?.Series.Count ?? 0;
            if (pm != null && seriesCount > index)
            {
                pm.Series[index].IsVisible = isVisible;
                pm.InvalidatePlot(true);   // View更新
            }
        }

        #endregion

        private ReactiveProperty<PlotModel> _OxyLineLevels;
        public ReactiveProperty<PlotModel> OxyLineLevels
        {
            get => _OxyLineLevels;
            private set => SetProperty(ref _OxyLineLevels, value);
        }

        public LineLevelsViewModel()
        {
            // 画素値の読み出し要求(Viewの表示中はずっと読み込んでおく)
            LineLevels.LoadImagePixels();

            OxyLineLevels = LineLevels
                .ObserveProperty(x => x.RgbLevelLine)
                .Select(x => GetPlotModelSkelton(x))
                .ToReactiveProperty();
        }

        private PlotModel GetPlotModelSkelton((byte R, byte G, byte B)[] rgb)
        {
            var pm = new PlotModel();
            //pm.Title = "Pixel Level";

            var rLine = new LineSeries { Color = OxyColors.Red };
            var gLine = new LineSeries { Color = OxyColors.Green };
            var bLine = new LineSeries { Color = OxyColors.Blue };

            if (rgb != null)
            {
                for (int i = 0; i < rgb.Length; i++)
                {
                    rLine.Points.Add(new DataPoint(i, rgb[i].R));
                    gLine.Points.Add(new DataPoint(i, rgb[i].G));
                    bLine.Points.Add(new DataPoint(i, rgb[i].B));
                }
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

    }
}
