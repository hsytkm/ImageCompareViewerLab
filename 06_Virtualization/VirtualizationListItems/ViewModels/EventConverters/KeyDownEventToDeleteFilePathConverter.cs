using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace VirtualizationListItems.ViewModels.EventConverters
{
    /// <summary>
    /// KeyDownイベント(dynamicは KeyEventArgs 想定)
    /// </summary>
    public class KeyDownEventToDeleteFilePathConverter : ReactiveConverter<dynamic, string>
    {
        protected override IObservable<string> OnConvert(IObservable<dynamic> source)
        {
            // DeleteKeyが押された時の選択アイテムのファイルPATHを通知
            return source
                //.Cast<KeyEventArgs>()
                .Where(_ => Keyboard.IsKeyDown(Key.Delete))
                .Select(_ => (AssociateObject as ListBox)?.SelectedItem)
                .Select(x => (x as ThubnailVModel)?.FilePath);
        }
    }
}
