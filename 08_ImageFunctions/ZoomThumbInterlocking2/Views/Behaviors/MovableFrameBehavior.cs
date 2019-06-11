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

        #region InterlockedField

        private readonly UniqueId MyInstanceId = new UniqueId();

        // staticにより全インスタンスで枠位置のシフト量を共有する
        private static readonly ReactivePropertySlim<InterlockedData<Vector>> InterlockedPointShiftRatio =
            new ReactivePropertySlim<InterlockedData<Vector>>(mode: ReactivePropertyMode.None);

        private void PulishInterlockedPointShiftRatio(Vector vector) =>
            InterlockedPointShiftRatio.Value = new InterlockedData<Vector>(MyInstanceId.Id, vector);

        // staticにより全インスタンスで枠サイズを共有する
        private static readonly ReactivePropertySlim<InterlockedData<Size>> InterlockedSizeChangeRatio =
            new ReactivePropertySlim<InterlockedData<Size>>(mode: ReactivePropertyMode.None);

        private void PulishInterlockedSizeChangeRatio(Size size) =>
            InterlockedSizeChangeRatio.Value = new InterlockedData<Size>(MyInstanceId.Id, size);

        #endregion

        private static bool IsSizeChanging => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

        private static readonly double DefaultLengthRatio = 0.1;
        private static readonly double DefaultAddrRatio = 0.5 - (DefaultLengthRatio / 2.0);
        private static readonly Size DefaultSizeRatio = new Size(DefaultLengthRatio, DefaultLengthRatio);
        private static readonly Point DefaultPointRatio = new Point(DefaultAddrRatio, DefaultAddrRatio);

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
                            //behavior.FrameAddrSizeRatio.Value = rectRatio;
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

            var frameNewSizeRatio = new ReactivePropertySlim<Size>(DefaultSizeRatio).AddTo(CompositeDisposable);
            var frameNewPointRatio = new ReactivePropertySlim<Point>(DefaultPointRatio).AddTo(CompositeDisposable);

            // マウスクリック移動による枠位置の更新
            AssociatedObject.MouseLeftDragAsObservable(originControl: parentPanel, handled: true)
                .CombineLatest(groundPanelSize, (mousePoint, groundSize) => (mousePoint, groundSize))
                .Subscribe(x =>
                {
                    if (IsSizeChanging)
                        frameNewSizeRatio.Value = GetSizeRatio(AssociatedObject, x.groundSize, x.mousePoint);
                    else
                        frameNewPointRatio.Value = GetPointRatio(AssociatedObject, x.groundSize, x.mousePoint);
                })
                .AddTo(CompositeDisposable);

            #region サイズ変更イベント

            // サイズ変更イベント
            frameNewSizeRatio
                .CombineLatest(groundPanelSize, (frameSizeRatio, groundSize) => (frameSizeRatio, groundSize))
                .Subscribe(x =>
                {
                    // 他コントロールへの通知
                    if (IsFrameInterlock) PulishInterlockedSizeChangeRatio(x.frameSizeRatio);

                    // 自コントロールの位置
                    UpdateFrameView(AssociatedObject, x.groundSize, x.frameSizeRatio);
                })
                .AddTo(CompositeDisposable);

            // インスタンス間のサイズの共有
            InterlockedSizeChangeRatio
                .Where(x => x.PublisherId != MyInstanceId.Id)
                .Select(x => x.Data)
                .CombineLatest(groundPanelSize, (frameSizeRatio, groundSize) => (frameSizeRatio, groundSize))
                .Subscribe(x =>
                {
                    UpdateFrameView(AssociatedObject, x.groundSize, x.frameSizeRatio);
                })
                .AddTo(CompositeDisposable);

            #endregion

            #region 位置変更イベント

            // 位置変更イベント
            frameNewPointRatio
                .CombineLatest(groundPanelSize, (framePointRatio, groundSize) => (framePointRatio, groundSize))
                .Subscribe(x =>
                {
                    // 他コントロールへの通知
                    if (IsFrameInterlock)
                    {
                        var pointVector = x.framePointRatio - FrameRectRatio.TopLeft;
                        if (pointVector != new Vector(0, 0))
                        {
                            PulishInterlockedPointShiftRatio(pointVector);
                        }
                    }

                    // 自コントロールの位置
                    UpdateFrameView(AssociatedObject, x.groundSize, x.framePointRatio);
                })
                .AddTo(CompositeDisposable);

            // インスタンス間のシフト量の共有
            InterlockedPointShiftRatio
                .Where(x => x.PublisherId != MyInstanceId.Id)
                .Select(x => x.Data)
                .CombineLatest(groundPanelSize, (shiftRatio, groundSize) => (shiftRatio, groundSize))
                .Subscribe(x =>
                {
                    var newPointRatio = FrameRectRatio.TopLeft + x.shiftRatio;
                    UpdateFrameView(AssociatedObject, x.groundSize, newPointRatio);
                })
                .AddTo(CompositeDisposable);

            #endregion

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            CompositeDisposable.Dispose();
        }

        #region UpdateFrameView

        // サイズのみ変更
        private void UpdateFrameView(FrameworkElement fe, Size groundSize, Size size) =>
            UpdateFrameView(fe, groundSize, new Rect(FrameRectRatio.TopLeft, size));

        // 位置のみ変更
        private void UpdateFrameView(FrameworkElement fe, Size groundSize, Point point) =>
            UpdateFrameView(fe, groundSize, new Rect(point, FrameRectRatio.Size));

        private void UpdateFrameView(FrameworkElement fe, Size groundSize, Rect rectRatio)
        {
            double clip(double value, double min, double max) => (value <= min) ? min : ((value >= max) ? max : value);

            var rect = rectRatio;
            rect.Scale(groundSize.Width, groundSize.Height);

            var width = clip(rect.Width, 0.0, groundSize.Width);
            var height = clip(rect.Height, 0.0, groundSize.Height);
            var left = clip(rect.X, 0.0, groundSize.Width - width);
            var top = clip(rect.Y, 0.0, groundSize.Height - height);

            Canvas.SetLeft(fe, left);
            Canvas.SetTop(fe, top);
            fe.Width = width;
            fe.Height = height;

            // プロパティの更新
            FrameRectRatio = new Rect(
                left / groundSize.Width, top / groundSize.Height,
                width / groundSize.Width, height / groundSize.Height);
        }

        #endregion

        #region FrameAddrSizeRatio

        // 枠のサイズ変更
        private static Size GetSizeRatio(FrameworkElement frameControl, Size groundSize, Vector shift)
        {
            var oldSize = ViewHelper.GetControlActualSize(frameControl);
            if (!oldSize.IsValidValue()) return default;

            var width = Math.Max(0, oldSize.Width + shift.X);
            var height = Math.Max(0, oldSize.Height + shift.Y);

            return new Size(width / groundSize.Width, height / groundSize.Height);
        }

        // Canvas上の位置を指定
        private static Point GetPointRatio(FrameworkElement frameControl, Size groundSize, Vector shift)
        {
            var oldPoint = new Point(Canvas.GetLeft(frameControl), Canvas.GetTop(frameControl));
            var newPoint = oldPoint + shift;

            return new Point(newPoint.X / groundSize.Width, newPoint.Y / groundSize.Height);
        }

        #endregion

    }
}
