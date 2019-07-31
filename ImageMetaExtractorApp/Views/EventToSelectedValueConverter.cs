using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;

namespace ImageMetaExtractorApp.Views
{
    class EventToSelectedValueConverter : ReactiveConverter<dynamic, object>
    {
        protected override IObservable<object> OnConvert(IObservable<dynamic> source)
        {
            return source.Select(_ => (AssociateObject as Selector)?.SelectedItem);
        }
    }
}
