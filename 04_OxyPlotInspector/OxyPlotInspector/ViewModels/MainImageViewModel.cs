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
    class MainImageViewModel : BindableBase
    {
        private readonly MainImageSource MainImage = ModelMaster.Instance.MainImage;
        private readonly Histogram Histogram = ModelMaster.Instance.Histogram;

        public ReadOnlyReactiveProperty<BitmapImage> ImageSource { get; }

        public LinePoints LinePoints { get; } = new LinePoints();

        #region MouseEvents

        public ReactiveProperty<Point> MouseDown { get; }
            = new ReactiveProperty<Point>(mode: ReactivePropertyMode.None);

        public ReactiveProperty<Unit> MouseUp { get; }
            = new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);

        public ReactiveProperty<Point> MouseMove { get; }
            = new ReactiveProperty<Point>(mode: ReactivePropertyMode.None);

        #endregion

        private readonly IContainerExtension _container;
        private readonly IRegionManager _regionManager;

        public MainImageViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;

            ImageSource = MainImage
                .ObserveProperty(x => x.ImageSource)
                .ToReadOnlyReactiveProperty();

            ImageSource.Subscribe(x => LinePoints.SetSourceSize(x.PixelWidth, x.PixelHeight));

            // マウス移動開始
            MouseDown.Subscribe(p => LinePoints.SetPoint1(p.X, p.Y));

            // マウス移動中
            var mouseMove = MouseDown.Merge(MouseDown.SelectMany(MouseMove.TakeUntil(MouseUp)));

            // ViewのLine表示は常時更新
            mouseMove.Subscribe(p => LinePoints.SetPoint2(p.X, p.Y));

            // Lineの画素値取得は重いので計算を間引く
            mouseMove
                .Throttle(TimeSpan.FromMilliseconds(500)) // 指定期間分だけ値が通過しなかったら最後の一つを流す
                .Subscribe(_ => Histogram.SetLinePointsRatio(LinePoints.GetPointsRatio()));

        }
    }

    class LinePoints : BindableBase
    {
        private bool _IsEnabled;
        public bool IsEnabled
        {
            get => _IsEnabled;
            private set => SetProperty(ref _IsEnabled, value);
        }

        public int SourceWidth { get; private set; }
        public int SourceHeight { get; private set; }

        public void SetSourceSize(int width, int height)
        {
            SourceWidth = width;
            SourceHeight = height;
        }

        #region X1,Y1,X2,Y2

        private double _X1;
        public double X1
        {
            get => _X1;
            private set => SetProperty(ref _X1, value);
        }

        private double _Y1;
        public double Y1
        {
            get => _Y1;
            private set => SetProperty(ref _Y1, value);
        }

        private double _X2;
        public double X2
        {
            get => _X2;
            private set => SetProperty(ref _X2, value);
        }

        private double _Y2;
        public double Y2
        {
            get => _Y2;
            private set => SetProperty(ref _Y2, value);
        }

        #endregion

        #region SetPoints

        public void SetPoint1(double x1, double y1)
        {
            X1 = X2 = x1;
            Y1 = Y2 = y1;
            IsEnabled = true;
        }

        public void SetPoint2(double x2, double y2)
        {
            X2 = x2;
            Y2 = y2;
        }

        #endregion

        // 線端の座標を割合で返す
        public (double X1, double Y1, double X2, double Y2) GetPointsRatio()
        {
            if (SourceWidth == 0) throw new DivideByZeroException(nameof(SourceWidth));
            if (SourceHeight == 0) throw new DivideByZeroException(nameof(SourceHeight));

            double limit(double x)
            {
                if (x < 0) return 0;
                if (x > 1) return 1;
                return x;
            }

            return (limit(X1 / SourceWidth), limit(Y1 / SourceHeight),
                limit(X2 / SourceWidth), limit(Y2 / SourceHeight));
        }

    }
}
