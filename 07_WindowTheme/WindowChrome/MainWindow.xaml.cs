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

    }
}

