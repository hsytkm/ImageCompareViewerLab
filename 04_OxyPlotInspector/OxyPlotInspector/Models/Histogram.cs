using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxyPlotInspector.Models
{
    class Histogram : BindableBase
    {
        public static Histogram Instance { get; } = new Histogram();
        private Histogram() { }

        // ヒストグラムViewの表示状態フラグ
        private bool _IsShowingHistgramView;
        public bool IsShowingHistgramView
        {
            get => _IsShowingHistgramView;
            set => SetProperty(ref _IsShowingHistgramView, value);
        }

        //private (double X1, double Y1, double X2, double Y2) LinePoints;

        public void SetLinePoints((double X1, double Y1, double X2, double Y2) points)
        {
            //LinePoints = points;

            double distance = Math.Sqrt(
                (points.X1 - points.X2) * (points.X1 - points.X2)
                + (points.Y1 - points.Y2) * (points.Y1 - points.Y2));

            Console.WriteLine(distance);
        }

    }
}
