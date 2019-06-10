using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

namespace ZoomThumb.Views.Common
{
    public static class MouseObservableExtensions
    {

        public static IObservable<MouseEventArgs> MouseLeaveAsObservable(this UIElement control)
            => Observable.FromEvent<MouseEventHandler, MouseEventArgs>
            (
                handler => (sender, e) => handler(e),
                handler => control.MouseLeave += handler,
                handler => control.MouseLeave -= handler
            );

        public static IObservable<MouseEventArgs> MouseLeftButtonDownAsObservableWithHandled(this UIElement control)
            => Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>
            (
                handler => (sender, e) =>
                {
                    handler(e);
                    e.Handled = true;
                },
                handler => control.MouseLeftButtonDown += handler,
                handler => control.MouseLeftButtonDown -= handler
            );

        public static IObservable<MouseEventArgs> MouseLeftButtonUpAsObservableWithHandled(this UIElement control)
            => Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>
            (
                handler => (sender, e) =>
                {
                    handler(e);
                    e.Handled = true;
                },
                handler => control.MouseLeftButtonUp += handler,
                handler => control.MouseLeftButtonUp -= handler
            );

        public static IObservable<MouseEventArgs> MouseMoveAsObservable(this UIElement control)
            => Observable.FromEvent<MouseEventHandler, MouseEventArgs>
            (
                handler => (sender, e) => handler(e),
                handler => control.MouseMove += handler,
                handler => control.MouseMove -= handler
            );

        /// <summary>
        /// マウスクリック中の移動量を流す
        /// </summary>
        /// <param name="control">対象コントロール</param>
        /// <param name="originControl">マウス移動量の原点コントロール</param>
        /// <returns>移動量</returns>
        public static IObservable<Vector> MouseLeftClickMoveAsObservable(this UIElement control, IInputElement originControl)
        {
            if (originControl is null) throw new ArgumentNullException(nameof(originControl));

            var mouseDown = control.MouseLeftButtonDownAsObservableWithHandled().ToUnit();
            var mouseUp = control.MouseLeftButtonUpAsObservableWithHandled().ToUnit();

            return control.MouseMoveAsObservable()
                .Select(e => e.GetPosition(originControl))
                .Pairwise().Select(x => x.NewItem - x.OldItem)
                .SkipUntil(mouseDown)
                .TakeUntil(mouseUp)
                .Repeat();
        }

    }
}
