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
using ThosoImage.Extensions;

namespace OxyPlotInspector.ViewModels
{
    class MainImageViewModel : BindableBase
    {
        private readonly MainImageSource MainImage = ModelMaster.Instance.MainImage;
        private readonly ImageLineLevels LineLevels = ModelMaster.Instance.LineLevels;

        public ReadOnlyReactiveProperty<BitmapImage> ImageSource { get; }

        public InspectLinePoints InspectLinePoints { get; } = new InspectLinePoints();

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

            ImageSource.Subscribe(x => InspectLinePoints.SetSourceSize(x.PixelWidth, x.PixelHeight));

            // Viewの非表示時のクリア
            LineLevels.ObserveProperty(x => x.IsShowingView).Where(b => !b)
                .Subscribe(_ =>
                {
                    InspectLinePoints.ClearPoints();    // 画像上のLine表示
                    LineLevels.ReleaseLinePoints();     // OxyPlot図(次回表示用)
                });

            // マウス移動開始
            MouseDown.Where(_ => LineLevels.IsShowingView)
                .Subscribe(p => InspectLinePoints.SetPoint1(p.X, p.Y));

            // マウス移動中
            var mouseMove = MouseDown.Merge(MouseDown.SelectMany(MouseMove.TakeUntil(MouseUp)))
                .Where(_ => LineLevels.IsShowingView);

            // ViewのLine表示は常時更新
            mouseMove.Subscribe(p => InspectLinePoints.SetPoint2(p.X, p.Y));

            // Line画素値の取得は重いので計算を間引く
            mouseMove
                .Throttle(TimeSpan.FromMilliseconds(500)) // 指定期間分だけ値が通過しなかったら最後の一つを流す
                .Subscribe(_ => LineLevels.SetLinePointsRatio(InspectLinePoints.GetPointsRatio()));

        }
    }

    class LinePoints : BindableBase
    {
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
        }

        public void SetPoint2(double x2, double y2)
        {
            X2 = x2;
            Y2 = y2;
        }

        #endregion

        public void ClearPoints() => X1 = X2 = Y1 = Y2 = 0.0;
    }

    class InspectLinePoints : LinePoints
    {
        private static readonly double ArrowRadian = 35.0 * Math.PI / 180.0;
        private static readonly double ArrowLengthMax = 6.0;

        #region LRPoints

        // 矢印の左側線
        private LinePoints _LeftWingPoints = new LinePoints();
        public LinePoints LeftWingPoints
        {
            get => _LeftWingPoints;
            private set => SetProperty(ref _LeftWingPoints, value);
        }

        // 矢印の右側線
        private LinePoints _RightWingPoints = new LinePoints();
        public LinePoints RightWingPoints
        {
            get => _RightWingPoints;
            private set => SetProperty(ref _RightWingPoints, value);
        }

        #endregion

        public int SourceWidth { get; private set; }
        private int SourceWidthMax => SourceWidth - 1;

        public int SourceHeight { get; private set; }
        private int SourceHeightMax => SourceHeight - 1;

        public void SetSourceSize(int width, int height)
        {
            SourceWidth = width;
            SourceHeight = height;
        }

        public new void SetPoint1(double x1, double y1)
        {
            x1 = x1.Limit(0, SourceWidthMax);
            y1 = y1.Limit(0, SourceHeightMax);

            base.SetPoint1(x1, y1);
            LeftWingPoints.SetPoint1(x1, y1);
            RightWingPoints.SetPoint1(x1, y1);
        }

        public new void SetPoint2(double x2, double y2)
        {
            x2 = x2.Limit(0, SourceWidthMax);
            y2 = y2.Limit(0, SourceHeightMax);

            base.SetPoint2(x2, y2);

            // 矢印の始点は解析線の先端
            LeftWingPoints.SetPoint1(x2, y2);
            RightWingPoints.SetPoint1(x2, y2);

            // 矢印の長さ
            var distance = Math.Sqrt((x2 - X1) * (x2 - X1) + (y2 - Y1) * (y2 - Y1));
            var length = Math.Min(distance, ArrowLengthMax);

            // 基準線上に載りP2寄りで回転させて矢印の端にする点を求める
            var radian = Math.Atan2(y2 - Y1, x2 - X1);
            LeftWingPoints.SetPoint2(
                x2 - length * Math.Cos(radian + ArrowRadian),
                y2 - length * Math.Sin(radian + ArrowRadian));
            RightWingPoints.SetPoint2(
                x2 - length * Math.Cos(radian - ArrowRadian),
                y2 - length * Math.Sin(radian - ArrowRadian));
        }

        public new void ClearPoints()
        {
            base.ClearPoints();
            LeftWingPoints.ClearPoints();
            RightWingPoints.ClearPoints();
        }

        // 線端の座標を割合で返す
        public (double X1, double Y1, double X2, double Y2) GetPointsRatio()
        {
            double limit(double x)
            {
                if (x <= 0D) return 0D;
                if (x >= 1D) return 1D;
                return x;
            }
            if (SourceWidthMax == 0) throw new DivideByZeroException(nameof(SourceWidthMax));
            if (SourceHeightMax == 0) throw new DivideByZeroException(nameof(SourceHeightMax));

            return (limit(X1 / SourceWidthMax), limit(Y1 / SourceHeightMax),
                limit(X2 / SourceWidthMax), limit(Y2 / SourceHeightMax));
        }

    }
}
