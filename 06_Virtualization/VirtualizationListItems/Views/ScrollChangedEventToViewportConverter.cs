using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using Reactive.Bindings.Interactivity;

namespace VirtualizationListItems.Views
{
    /// <summary>
    /// ScrollChangedイベント
    /// </summary>
    public class ScrollChangedEventToViewportConverter : ReactiveConverter<dynamic, (double, double)>
    {
        protected override IObservable<(double, double)> OnConvert(IObservable<dynamic> source)
        {
            return source
                .Cast<ScrollChangedEventArgs>()
                .Where(e => !(e.ViewportWidthChange == 0 && e.ViewportHeightChange == 0) || e.HorizontalChange != 0)
                .Select(e => (CenterRatio: CenterRatio(e.ExtentWidth, e.ViewportWidth, e.HorizontalOffset),
                    ViewportRatio: ViewportRatio(e.ExtentWidth, e.ViewportWidth)));
        }

        /// <summary>
        /// 表示範囲の中央の割合
        /// </summary>
        private static double CenterRatio(double length, double viewport, double offset)
        {
            if (length == 0) return 0;

            double d = (offset + (viewport / 2)) / length;
            return Math.Max(0, Math.Min(1, d));
        }

        /// <summary>
        /// 全要素と表示範囲の割合(要素が全て表示されていたら1.0)
        /// </summary>
        private static double ViewportRatio(double length, double viewport)
        {
            if (length == 0) return 0;

            double d  = viewport / length;
            return Math.Max(0, Math.Min(1, d));
        }
    }
}
