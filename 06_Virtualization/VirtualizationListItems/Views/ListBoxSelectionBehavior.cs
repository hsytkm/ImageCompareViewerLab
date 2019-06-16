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

        /// <summary>
        /// ユーザに選択された項目までスクロール
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ListBox listBox)) return;
            listBox.ScrollIntoView(listBox.SelectedItem);
        }

    }
}
