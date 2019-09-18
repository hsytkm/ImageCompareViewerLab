using System.Windows.Controls;
using System.Windows.Interactivity;

namespace VirtualizationListItems.Views
{
    class ListBoxSelectionBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }

        // 選択中アイテムの番号バッファ(選択中アイテムが消されたときの再選択用)
        private int _selectedIndexBuffer;

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ListBox listBox)) return;

            // 選択中アイテムが消されたときの再選択
            if (listBox.SelectedItem == null && listBox.Items.Count > 0)
            {
                var max = listBox.Items.Count - 1;
                var newIndex = _selectedIndexBuffer < 0 ? 0 : (_selectedIndexBuffer < max ? _selectedIndexBuffer : max);
                listBox.SelectedItem = listBox.Items[newIndex];
            }
            _selectedIndexBuffer = listBox.SelectedIndex;

            // 選択項目まで表示をスクロール
            listBox.ScrollIntoView(listBox.SelectedItem);
        }

    }
}
