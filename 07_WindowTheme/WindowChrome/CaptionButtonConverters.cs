using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace WindowChrome
{
    /// <summary>
    ///        ウィンドウのリサイズモードに応じて最大化・最小化ボタンの表示・非表示を設定します。
    /// </summary>
    //public sealed class ResizeCaptionButtonVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        // ResizeModeがNoResizeの時はVisibility.Collapsedに、それ以外の時はVisibility.Visibleにします。
    //        (value is ResizeMode && (ResizeMode)value != ResizeMode.NoResize) ? Visibility.Visible : Visibility.Collapsed;

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        throw new NotImplementedException();
    //}

    /// <summary>
    ///        ウィンドウのリサイズモードに応じて最大化ボタンの有効・無効を設定します。
    /// </summary>
    //public sealed class MaximizeCaptionButtonEnableConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        // ResizeModeがCanMinimizeの時はfalseに、それ以外の時はtrueにします。
    //        (value is ResizeMode && (ResizeMode)value != ResizeMode.CanMinimize);

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        throw new NotImplementedException();
    //}

    /// <summary>
    ///        ウィンドウサイズに応じて、最大化ボタンの表示文字列を設定します。
    /// </summary>
    public sealed class MaximizeCaptionButtonContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            // Malettフォントでは「1」が「最大化」、「2」が「元に戻す」です。
            (value is WindowState && (WindowState)value == WindowState.Maximized) ? "2" : "1";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    /// <summary>
    ///        ウィンドウサイズに応じて、最大化ボタンのツールチップを設定します。
    /// </summary>
    //public sealed class MaximizeCaptionButtonTooltipConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        (value is WindowState && (WindowState)value == WindowState.Maximized) ? "元に戻す" : "最大化";

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        throw new NotImplementedException();
    //}

    /// <summary>
    /// 最大化時にアプリがタスクバーにめり込まないようにMarginを設定する
    /// </summary>
    public sealed class RootGridMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value is WindowState && (WindowState)value == WindowState.Maximized)
                ? "9,5,5,9" : "0";
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

}
