using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using SwitchContext.Common;
using SwitchContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ThosoImage.Extensions;

namespace SwitchContext.ViewModels
{
    /// <summary>
    /// 画像表示の基底ViewModel
    /// </summary>
    class MultiImageViewModelBase : BindableBase, IActiveAware
    {
        public virtual int ContentCount { get; } = 0;
        public virtual string ImageContentRegion { get; } = "Region";
        public virtual string Title { get; } = "Multi";

        private readonly MainImages MainImages = ModelContext.Instance.MainImages;

        private readonly IRegionManager _regionManager;
        private readonly IApplicationCommands _applicationCommands;

        public DelegateCommand SwapImagesInnerCommand { get; }
        public DelegateCommand SwapImagesOuterCommand { get; }

        public MultiImageViewModelBase(IRegionManager regionManager, IApplicationCommands applicationCommands)
        {
            _regionManager = regionManager;
            _applicationCommands = applicationCommands;

            SwapImagesInnerCommand = new DelegateCommand(SwapImageViewModelsInnerTrack);
            _applicationCommands.SwapInnerTrackCommand.RegisterCommand(SwapImagesInnerCommand);

            SwapImagesOuterCommand = new DelegateCommand(SwapImageViewModelsOuterTrack);
            _applicationCommands.SwapOuterTrackCommand.RegisterCommand(SwapImagesOuterCommand);

            IsActiveChanged += DoubleImageTabItemViewModel_IsActiveChanged;
        }

        // Region内のViewsを取得
        private IEnumerable<FrameworkElement> GetRegionViews() =>
            _regionManager.Regions[ImageContentRegion].Views.Cast<FrameworkElement>();

        // アクティブ状態変化時の処理
        private void DoubleImageTabItemViewModel_IsActiveChanged(object sender, EventArgs e)
        {
            if (!(e is DataEventArgs<bool> e2)) return;

            if (e2.Value)
            {
                // アクティブ化時
                foreach (var (item, index) in GetRegionViews().WithIndex())
                {
                    if (item.DataContext is ImagePanelViewModel vm)
                        vm.UpdateImageSource(index);
                }
            }
            else
            {
                // 非アクティブ化時
                MainImages.AdaptImageListTracks(ContentCount);
            }
        }

        // 画像(ViewModel)を内回りで入れ替え
        private void SwapImageViewModelsInnerTrack()
        {
            if (ContentCount <= 1) return;  // 回転する必要なし
            var views = GetRegionViews();

            var tail = views.Last().DataContext;
            for (int i = views.Count() - 1; i > 0 ; i--)
            {
                views.ElementAt(i).DataContext = views.ElementAt(i - 1).DataContext;
            }
            views.First().DataContext = tail;

            MainImages.RotateInnerTrack();
        }

        // 画像(ViewModel)を外回りで入れ替え
        private void SwapImageViewModelsOuterTrack()
        {
            if (ContentCount <= 1) return;  // 回転する必要なし
            var views = GetRegionViews();

            var head = views.First().DataContext;
            for (int i = 0; i < views.Count() - 1; i++)
            {
                views.ElementAt(i).DataContext = views.ElementAt(i + 1).DataContext;
            }
            views.Last().DataContext = head;

            MainImages.RotateOuterTrack();
        }

        #region IActiveAware

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (SetProperty(ref _isActive, value))
                {
                    SwapImagesInnerCommand.IsActive = value;
                    SwapImagesOuterCommand.IsActive = value;
                    IsActiveChanged?.Invoke(this, new DataEventArgs<bool>(value));
                }
            }
        }

        public event EventHandler IsActiveChanged;

        #endregion

    }
}
