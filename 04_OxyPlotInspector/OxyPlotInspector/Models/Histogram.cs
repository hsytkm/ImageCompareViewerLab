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


    }
}
