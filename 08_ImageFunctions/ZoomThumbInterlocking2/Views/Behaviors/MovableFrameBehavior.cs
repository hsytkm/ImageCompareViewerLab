using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using ZoomThumb.Common;
using ZoomThumb.Views.Common;

namespace ZoomThumb.Views.Behaviors
{
    class MovableFrameBehavior : Behavior<FrameworkElement>
    {
        private static readonly Type SelfType = typeof(MovableFrameBehavior);
        private static bool IsSizeChanging => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

        private static readonly double DefaultSizeRatio = 0.1;
        private static readonly double DefaultAddrRatio = 0.5 - (DefaultSizeRatio / 2.0);

        private readonly ReactivePropertySlim<Rect> FrameAddrSizeRatio =
            new ReactivePropertySlim<Rect>(initialValue: new Rect(DefaultAddrRatio, DefaultAddrRatio, DefaultSizeRatio, DefaultSizeRatio));

        private readonly ReactivePropertySlim<Unit> MouseLeftDown = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None);
        private readonly ReactivePropertySlim<Unit> MouseLeftUp = new ReactivePropertySlim<Unit>(mode: ReactivePropertyMode.None);
        private readonly ReactivePropertySlim<Point> MouseMove = new ReactivePropertySlim<Point>(mode: ReactivePropertyMode.None);
        private readonly ReactivePropertySlim<Size> GroundPanelSize = new ReactivePropertySlim<Size>(mode: ReactivePropertyMode.None);

        private readonly MyCompositeDisposable CompositeDisposable = new MyCompositeDisposable();

        #region FrameRectRatioProperty(TwoWay)

        // View枠の割合
        private static readonly DependencyProperty FrameRectRatioProperty =
            DependencyProperty.Register(
                nameof(FrameRectRatio),
                typeof(Rect),
                SelfType,
                new FrameworkPropertyMetadata(
                    default(Rect),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) =>
                    {
                        if (d is MovableFrameBehavior behavior && e.NewValue is Rect rectRatio)
                        {
                            behavior.FrameAddrSizeRatio.Value = rectRatio;
                        }
                    }));

        public Rect FrameRectRatio
        {
            get => (Rect)GetValue(FrameRectRatioProperty);
            set => SetValue(FrameRectRatioProperty, value);
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;

            // 親パネル
            if (VisualTreeHelper.GetParent(AssociatedObject) is Panel parentPanel)
            {
                parentPanel.SizeChanged += ParentPanel_SizeChanged;
            }

            #region ReactiveProperties

            // 枠のマウス操作
            MouseMove
                .Pairwise().Select(x => x.NewItem - x.OldItem)
                .SkipUntil(MouseLeftDown)
                .TakeUntil(MouseLeftUp)
                .Repeat()
                .Subscribe(vector =>
                {
                    var groundSize = GroundPanelSize.Value;

                    var rect = IsSizeChanging
                        ? GetFrameRectFromSizeShift(AssociatedObject, groundSize, vector) 
                        : GetFrameRectFromAddrShift(AssociatedObject, groundSize, vector);

                    FrameAddrSizeRatio.Value = rect;
                })
                .AddTo(CompositeDisposable);

            // 枠の描画更新
            FrameAddrSizeRatio
                .CombineLatest(GroundPanelSize, (rectRate, groundSize) => (rectRate, groundSize))
                .Subscribe(x =>
                {
                    //Console.WriteLine($"FrameRect: {x.frameRect.X:f3}  {x.frameRect.Y:f3}  {x.frameRect.Width:f3}  {x.frameRect.Height:f3} ");

                    var rect = x.rectRate;
                    rect.Scale(x.groundSize.Width, x.groundSize.Height);

                    Canvas.SetLeft(AssociatedObject, rect.X);
                    Canvas.SetTop(AssociatedObject, rect.Y);
                    AssociatedObject.Width = rect.Width;
                    AssociatedObject.Height = rect.Height;

                    FrameRectRatio = x.rectRate;
                })
                .AddTo(CompositeDisposable);

            #endregion

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;

            if (VisualTreeHelper.GetParent(AssociatedObject) is Panel parentPanel)
            {
                parentPanel.SizeChanged -= ParentPanel_SizeChanged;
            }

            CompositeDisposable.Dispose();
        }

        #region ParentPanel

        // 親パネルのサイズ変更時に枠が食み出ないように制限する
        private void ParentPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!(sender is FrameworkElement fe)) return;
            GroundPanelSize.Value = ViewHelper.GetControlActualSize(fe);
        }

        #endregion

        #region MouseMove

        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseLeftDown.Value = Unit.Default;
            e.Handled = true;
        }

        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseLeftUp.Value = Unit.Default;
            e.Handled = true;
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is DependencyObject d)) return;

            // サイズ変更:斜め両矢印 / 位置変更:両矢印の十字
            Window.GetWindow(d).Cursor = IsSizeChanging ? Cursors.SizeNWSE : Cursors.SizeAll;

            // 移動させるコントロール基準にすると移動により相対的にマウス位置が変化して
            // ハンチングするので親パネルを基準にする
            if (VisualTreeHelper.GetParent(d) is Panel panel)
            {
                MouseMove.Value = e.GetPosition((IInputElement)panel);
            }
        }

        private static void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!(sender is DependencyObject d)) return;

            // 通常マウス(左上向き矢印)に戻す
            Window.GetWindow(d).Cursor = Cursors.Arrow;
        }


        #endregion

        #region FrameAddrSizeRatio

        // Canvas上の位置を指定
        private static Rect GetFrameRectFromAddrShift(FrameworkElement frameControl, Size groundSize, Vector shift)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            var currentActualSize = ViewHelper.GetControlActualSize(frameControl);
            if (!currentActualSize.IsValidValue()) return default;

            var oldPoint = new Point(Canvas.GetLeft(frameControl), Canvas.GetTop(frameControl));
            var newPoint = oldPoint + shift;

            var left = clip(newPoint.X, 0.0, groundSize.Width - currentActualSize.Width);
            var top = clip(newPoint.Y, 0.0, groundSize.Height - currentActualSize.Height);

            return new Rect(
                left / groundSize.Width,
                top / groundSize.Height,
                currentActualSize.Width / groundSize.Width,
                currentActualSize.Height / groundSize.Height);
        }

        // 枠のサイズ変更
        private static Rect GetFrameRectFromSizeShift(FrameworkElement frameControl, Size groundSize, Vector shift)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            var currentPoint = new Point(Canvas.GetLeft(frameControl), Canvas.GetTop(frameControl));

            var oldSize = ViewHelper.GetControlActualSize(frameControl);
            if (!oldSize.IsValidValue()) return default;

            var width = clip(oldSize.Width + shift.X, 0.0, groundSize.Width - currentPoint.X);
            var height = clip(oldSize.Height + shift.Y, 0.0, groundSize.Height - currentPoint.Y);

            return new Rect(
                currentPoint.X / groundSize.Width,
                currentPoint.Y / groundSize.Height,
                width / groundSize.Width,
                height / groundSize.Height);
        }

        #endregion

    }
}
