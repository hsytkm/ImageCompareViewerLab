using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace ZoomThumb.ViewModels.EventConverters
{
    class VisibilityChangedToBoolConverter : ReactiveConverter<dynamic, bool>
    {
        protected override IObservable<bool> OnConvert(IObservable<dynamic> source)
        {
            return source.Select(e => (bool)(e.NewValue == Visibility.Visible));
        }
    }

}
