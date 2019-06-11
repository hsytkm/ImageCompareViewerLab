using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
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
            new ReactivePropertySlim<Rect>(new Rect(DefaultAddrRatio, DefaultAddrRatio, DefaultSizeRatio, DefaultSizeRatio));

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

        #region IsFrameInterlockProperty(OneWay)

        // スクロール/サイズを他コントロールと連動
        private static readonly DependencyProperty IsFrameInterlockProperty =
            DependencyProperty.Register(
                nameof(IsFrameInterlock),
                typeof(bool),
                SelfType,
                new FrameworkPropertyMetadata(
                    default(bool),
                    FrameworkPropertyMetadataOptions.None));

        public bool IsFrameInterlock
        {
            get => (bool)GetValue(IsFrameInterlockProperty);
            set => SetValue(IsFrameInterlockProperty, value);
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            var parentPanel = VisualTreeHelper.GetParent(AssociatedObject) as Panel;
            if (parentPanel is null) throw new ResourceReferenceKeyNotFoundException();

            // マウスポインタ変更(サイズ変更:斜め両矢印 / 位置変更:両矢印の十字)
            AssociatedObject.MouseMoveAsObservable()
                .Subscribe(_ => Window.GetWindow(AssociatedObject).Cursor = IsSizeChanging ? Cursors.SizeNWSE : Cursors.SizeAll)
                .AddTo(CompositeDisposable);

            // マウスポインタを通常(左上向き矢印)に戻す
            AssociatedObject.MouseLeaveAsObservable()
                .Subscribe(_ => Window.GetWindow(AssociatedObject).Cursor = Cursors.Arrow)
                .AddTo(CompositeDisposable);

            // 親パネルのサイズ取得
            var groundPanelSize = parentPanel.SizeChangedAsObservable().Select(e => e.NewSize)
                .ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.DistinctUntilChanged)
                .AddTo(CompositeDisposable);

            // マウスクリック移動による枠位置の更新
            AssociatedObject.MouseLeftDragAsObservable(originControl: parentPanel, handled: true)
                .Subscribe(v => FrameAddrSizeRatio.Value = GetNewFrameAddrSizeRatio(AssociatedObject, groundPanelSize.Value, v))
                .AddTo(CompositeDisposable);

            // 枠の描画更新
            FrameAddrSizeRatio
                .CombineLatest(groundPanelSize, (rectRate, groundSize) => (rectRate, groundSize))
                .Subscribe(x =>
                {
                    //Console.WriteLine($"FrameRect: {x.frameRect.X:f3}  {x.frameRect.Y:f3}  {x.frameRect.Width:f3}  {x.frameRect.Height:f3} ");
                    var rect = x.rectRate;
                    rect.Scale(x.groundSize.Width, x.groundSize.Height);

                    Canvas.SetLeft(AssociatedObject, rect.X);
                    Canvas.SetTop(AssociatedObject, rect.Y);
                    AssociatedObject.Width = rect.Width;
                    AssociatedObject.Height = rect.Height;

                    // プロパティの更新
                    FrameRectRatio = x.rectRate;
                })
                .AddTo(CompositeDisposable);

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            CompositeDisposable.Dispose();
        }

        #region FrameAddrSizeRatio

        private static Rect GetNewFrameAddrSizeRatio(FrameworkElement frameControl, Size groundSize, Vector shift) =>
            IsSizeChanging
                ? GetFrameAddrSizeFromSizeShift(frameControl, groundSize, shift)
                : GetFrameAddrSizeFromAddrShift(frameControl, groundSize, shift);

        // 枠のサイズ変更
        private static Rect GetFrameAddrSizeFromSizeShift(FrameworkElement frameControl, Size groundSize, Vector shift)
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

        // Canvas上の位置を指定
        private static Rect GetFrameAddrSizeFromAddrShift(FrameworkElement frameControl, Size groundSize, Vector shift)
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

        #endregion

    }
}
