using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ImageMetaExtractorApp.Views
{
    class SelectorMouseWheelBehavior : Behavior<Selector>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseWheel += AssociatedObject_MouseWheel;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseWheel -= AssociatedObject_MouseWheel;
        }

        /// <summary>
        /// マウスホイールで選択項目を移動する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AssociatedObject_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!(sender is Selector selector)) return;
            if (!selector.HasItems) return;

            var index = selector.SelectedIndex + ((e.Delta > 0) ? -1 : 1);
            if (index < 0)
            {
                index = 0;
            }
            else
            {
                var max = selector.Items.Count - 1;
                if (index > max)
                {
                    index = max;
                }
            }
            selector.SelectedIndex = index;
        }

    }
}
