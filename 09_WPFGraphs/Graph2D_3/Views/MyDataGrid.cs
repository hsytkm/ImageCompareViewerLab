using Graph2D.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Graph2D.Views
{
    // WPF DataGrid ItemsSourceの更新時の取得方法 https://stackoverrun.com/ja/q/2836802
    public class MyDataGrid : DataGrid
    {
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (!(newValue is IEnumerable<ColoredObjectRow> newItems)) return;

            var col = newItems.First().ItemsSource.Count;
            Debug.WriteLine($"Row={newItems.Count()}, Col={col}");

            this.Columns.Clear();
            for (int c = 0; c < col; c++)
            {
                var bindingTarget = $"{nameof(ColoredObjectRow.ItemsSource)}[{c}].";

                var style = new Style(typeof(TextBlock));

                style.Setters.Add(new Setter(TextBlock.BackgroundProperty,
                    new Binding($"{bindingTarget}{nameof(ColoredObject.Background)}") { Mode = BindingMode.OneTime }));

                style.Setters.Add(new Setter(TextBlock.ForegroundProperty,
                    new Binding($"{bindingTarget}{nameof(ColoredObject.Foreground)}") { Mode = BindingMode.OneTime }));

                this.Columns.Add(new DataGridTextColumn()
                {
                    Binding = new Binding($"{bindingTarget}{nameof(ColoredObject.Object)}") { Mode = BindingMode.OneTime },
                    ElementStyle = style,
                });
            }
        }

    }
}
