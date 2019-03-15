using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace ZoomThumb.ViewModels.EventConverters
{
    /// <summary>
    /// MouseWheelイベント(dynamicは MouseWheelEventArgs 想定)
    /// </summary>
    public class MouseWheelEventToDeltaConverter : ReactiveConverter<dynamic, int>
    {
        protected override IObservable<int> OnConvert(IObservable<dynamic> source)
        {
            return source.Select(e => (int)e.Delta);
        }
    }
}
