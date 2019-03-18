using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace ZoomThumb.ViewModels.EventConverters
{
    /// <summary>
    /// MouseWheelイベント(dynamicは MouseWheelEventArgs 想定)
    /// </summary>
    class MouseWheelCtrlEventToDeltaConverter : ReactiveConverter<dynamic, int>
    {
        protected override IObservable<int> OnConvert(IObservable<dynamic> source)
        {
            return source.Cast<MouseWheelEventArgs>()
                .Select(e =>
                {
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        // 最大ズームでホイールすると画像の表示エリアが移動しちゃうので止める
                        e.Handled = true;
                        return e.Delta;
                    }
                    return 0;
                });
        }
    }
}
