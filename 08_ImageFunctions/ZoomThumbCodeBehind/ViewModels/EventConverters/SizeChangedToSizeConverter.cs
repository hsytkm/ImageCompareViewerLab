using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace ZoomThumb.ViewModels.EventConverters
{
    /// <summary>
    /// SizeChangedイベント(dynamicは SizeChangedEventArgs 想定)
    /// </summary>
    class SizeChangedToSizeConverter : ReactiveConverter<dynamic, Size>
    {
        protected override IObservable<Size> OnConvert(IObservable<dynamic> source)
        {
            return source.Select(e => new Size((double)e.NewSize.Width, (double)e.NewSize.Height));
        }
    }
}
