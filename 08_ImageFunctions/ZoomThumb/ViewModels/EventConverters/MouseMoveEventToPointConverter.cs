using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace ZoomThumb.ViewModels.EventConverters
{
    // EventToReactiveProperty converter.
    // Converter/IgnoreEventArgs is useful for unit testings.
    // For example, MouseMove.Value = new Point(10, 10) is simulate MouseMove
    // MouseEnter.Value = new Unit() is simulate raise MouseEnter event.
    public class MouseMoveEventToPointConverter : ReactiveConverter<dynamic, Point>
    {
        protected override IObservable<Point> OnConvert(IObservable<dynamic> source)
        {
            return source
                .Select(x => x.GetPosition((IInputElement)AssociateObject))
                .Cast<Point>();
        }
    }
}
