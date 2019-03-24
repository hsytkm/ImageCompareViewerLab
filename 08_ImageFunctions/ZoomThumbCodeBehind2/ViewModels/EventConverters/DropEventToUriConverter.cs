using Reactive.Bindings.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace ZoomThumb.ViewModels.EventConverters
{
    /// <summary>
    /// Dropイベント(dynamicはDragEventArgs想定)
    /// </summary>
    public class DropEventToUriConverter : ReactiveConverter<dynamic, IEnumerable<Uri>>
    {
        protected override IObservable<IEnumerable<Uri>> OnConvert(IObservable<dynamic> source)
        {
            return source
                .Cast<DragEventArgs>()
                .Select(e => ToUrl(e.Data));
        }

        private static IEnumerable<Uri> ToUrl(IDataObject data)
        {
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                return (data.GetData(DataFormats.FileDrop) as string[])
                    .Select(s => new Uri(s));
            }
            else
            {
                var uri = new Uri(data.GetData(DataFormats.Text).ToString());
                return new List<Uri>() { uri };
            }
        }
    }
}
