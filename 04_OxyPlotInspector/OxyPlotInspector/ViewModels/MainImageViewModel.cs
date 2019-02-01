﻿using OxyPlotInspector.Models;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using ThosoImage.Wpf.Imaging;

namespace OxyPlotInspector.ViewModels
{
    class MainImageViewModel : BindableBase
    {
        private readonly string ImageSourcePath = @"C:/data/Image1.jpg";
        private readonly int ImageSourceWidth = 320;
        private readonly int ImageSourceHeight = 240;

        private readonly Histogram Histogram = Histogram.Instance;

        public BitmapImage ImageSource { get; }

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

            // サイズを指定して画像を読み込み
            ImageSource = ImageSourcePath.ToBitmapImage(ImageSourceWidth, ImageSourceHeight);

            // MoveStart
            MouseDown.Subscribe(p => LinePoints.SetPoint1(p.X, p.Y));

            // Moving
            MouseDown.Merge(MouseDown.SelectMany(MouseMove.TakeUntil(MouseUp)))
                .Subscribe(p => {
                    LinePoints.SetPoint2(p.X, p.Y);
                    Histogram.SetLinePoints(LinePoints.GetPoints());
                });

        }

    }

    class LinePoints : BindableBase
    {
        public bool IsEnabled { get; private set; }

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

        public (double X1, double Y1, double X2, double Y2) GetPoints() => (X1, Y1, X2, Y2);

    }
}
