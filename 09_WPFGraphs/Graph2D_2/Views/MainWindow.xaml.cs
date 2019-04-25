using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Graph2D.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataGridCell_Loaded(object sender, RoutedEventArgs e)
        {
            SolidColorBrush ToSolidColorBrush(string s) =>
                new SolidColorBrush((Color)new ColorConverter().ConvertFrom(s));

            if (!(sender is ContentControl cc)) return;
            if (!(cc.Content is TextBlock textBlock)) return;

            var data = textBlock.Text.Split(',');
            if (data.Length != 3) return;

            textBlock.Foreground = ToSolidColorBrush(data[1]);
            textBlock.Background = ToSolidColorBrush(data[2]);
            textBlock.Text = data[0];

            textBlock.TextAlignment = TextAlignment.Right;
        }

    }
}
