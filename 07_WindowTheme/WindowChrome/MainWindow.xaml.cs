using System.Windows;
using System.Windows.Input;

namespace WindowChrome
{
    /// <summary>
    ///        MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region キャプションボタンのイベント

        /// <summary>
        ///        閉じる及び、終了ボタンを押した時のイベントです。
        /// </summary>
        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            // アプリを終了します。
            SystemCommands.CloseWindow(this);
        }

        /// <summary>
        ///        最小化ボタンを押した時のイベントです。
        /// </summary>
        private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            // ウインドウを最小化します。
            SystemCommands.MinimizeWindow(this);
        }

        /// <summary>
        ///        最大化ボタンを押した時のイベントです。
        /// </summary>
        private void MaximizeOrRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            // ウィンドウを最大化（ もしくは元のサイズに ）します。
            if (WindowState != WindowState.Maximized)
                SystemCommands.MaximizeWindow(this);
            else
                SystemCommands.RestoreWindow(this);
        }

        #endregion

        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    // 右上の×ボタンを押したいので上と右はマージンなしにしてみる
                    LayoutRoot.Margin = new Thickness(9, 0, 0, 9);
                    break;
                default:
                    LayoutRoot.Margin = new Thickness(0);
                    break;
            }
        }

    }
}

