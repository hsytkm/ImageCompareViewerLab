using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ZoomThumb.Views.Common
{
    public static class UIEventObservableExtensions
    {

        public static IObservable<SizeChangedEventArgs> SizeChangedAsObservable(this FrameworkElement control)
            => Observable.FromEvent<SizeChangedEventHandler, SizeChangedEventArgs>
            (
                handler => (sender, e) => handler(e),
                handler => control.SizeChanged += handler,
                handler => control.SizeChanged -= handler
            );

        public static IObservable<DataTransferEventArgs> TargetUpdatedAsObservable(this FrameworkElement control)
            => Observable.FromEvent<EventHandler<DataTransferEventArgs>, DataTransferEventArgs>
            (
                handler => (sender, e) => handler(e),
                handler => control.TargetUpdated += handler,
                handler => control.TargetUpdated -= handler
            );

        public static IObservable<ScrollChangedEventArgs> ScrollChangedAsObservable(this ScrollViewer control)
            => Observable.FromEvent<ScrollChangedEventHandler, ScrollChangedEventArgs>
            (
                handler => (sender, e) => handler(e),
                handler => control.ScrollChanged += handler,
                handler => control.ScrollChanged -= handler
            );

        public static IObservable<DragDeltaEventArgs> DragDeltaAsObservable(this Thumb control)
            => Observable.FromEvent<DragDeltaEventHandler, DragDeltaEventArgs>
            (
                handler => (sender, e) => handler(e),
                handler => control.DragDelta += handler,
                handler => control.DragDelta -= handler
            );

    }
}
