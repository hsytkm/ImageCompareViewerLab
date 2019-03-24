using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace ZoomThumb.ViewModels.EventConverters
{
    class LoadedToSizeConverter : ReactiveConverter<dynamic, Size>
    {
        protected override IObservable<Size> OnConvert(IObservable<dynamic> source)
        {
            if (!(AssociateObject is FrameworkElement fe)) throw new ArgumentException();
            return source.Select(_ => new Size(fe.ActualWidth, fe.ActualHeight));
        }
    }
}
