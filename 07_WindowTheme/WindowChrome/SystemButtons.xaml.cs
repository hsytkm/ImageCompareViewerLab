using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowChrome
{
    /// <summary>
    /// SystemButtons.xaml の相互作用ロジック
    /// </summary>
    public partial class SystemButtons : UserControl
    {
        private Window Window
        {
            get
            {
                if (_window is null)
                    _window = Window.GetWindow(this);
                return _window;
            }
        }
        private Window _window;

        public SystemButtons()
        {
            InitializeComponent();
        }

        /// <summary>
        /// アプリを終了します
        /// </summary>
        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(Window);
        }

        /// <summary>
        /// ウインドウを最小化します
        /// </summary>
        private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(Window);
        }

        /// <summary>
        /// ウィンドウを最大化（ もしくは元のサイズに ）します
        /// </summary>
        private void MaximizeOrRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (Window.WindowState != WindowState.Maximized)
                SystemCommands.MaximizeWindow(Window);
            else
                SystemCommands.RestoreWindow(Window);
        }

    }
}
