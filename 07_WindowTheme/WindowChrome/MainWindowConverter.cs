using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;

namespace WindowChrome
{
    /// <summary>
    ///        ウィンドウのアクティブ状態に合わせて、Border.BorderBrush値を調節します。
    /// </summary>
    //public sealed class WindowBorderBrushConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        (value is bool && (bool)value) ? parameter as SolidColorBrush : Brushes.LightGray;

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        throw new NotImplementedException();
    //}

    /// <summary>
    ///        ウィンドウサイズに応じて、Border.Thicknessの値を調節します。
    /// </summary>
    //public sealed class BorderThicknessByWindowStateConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        // 最大化時にBorder.Thicknessの値を8.0に設定することで、コントロールが画面の外にはみ出ないようにします。
    //        (value is WindowState && (WindowState)value == WindowState.Maximized) ? 8.0 : 1.0;

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        throw new NotImplementedException();
    //}

    /// <summary>
    ///        ウィンドウのアクティブ状態に合わせて、タイトルバーの文字色を調節します。
    /// </summary>
    //public sealed class CaptionForegroundConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        (value is bool && (bool)value) ? parameter as SolidColorBrush : MyColors.ForegroundInActive;

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        throw new NotImplementedException();
    //}

    /// <summary>
    ///        ウィンドウのアクティブ状態に合わせて、タイトルバーのBackground値を調節します。
    /// </summary>
    //public sealed class CaptionBackgroundConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        (value is bool && (bool)value) ? parameter as SolidColorBrush : MyColors.BackgroundInActive;

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        throw new NotImplementedException();
    //}

}
