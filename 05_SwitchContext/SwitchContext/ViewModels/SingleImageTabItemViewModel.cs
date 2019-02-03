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
    class SingleImageTabItemViewModel : BindableBase, IActiveAware
    {
        public string Title { get; } = "Single";
        private static readonly string ImageContentRegion = "Image1ContentRegion";

        private readonly MainImages MainImages = ModelContext.Instance.MainImages;

        private readonly IRegionManager _regionManager;
        private readonly IApplicationCommands _applicationCommands;
        public DelegateCommand SwapImagesCommand { get; }

        public SingleImageTabItemViewModel(IRegionManager regionManager, IApplicationCommands applicationCommands)
        {
            _regionManager = regionManager;
            _applicationCommands = applicationCommands;

            SwapImagesCommand = new DelegateCommand(SwapImageViewModels);

            _applicationCommands.SaveCommand.RegisterCommand(SwapImagesCommand);

            IsActiveChanged += DoubleImageTabItemViewModel_IsActiveChanged;
        }

        private IEnumerable<FrameworkElement> GetRegionViews() =>
            _regionManager.Regions[ImageContentRegion].Views.Cast<FrameworkElement>();

        private void DoubleImageTabItemViewModel_IsActiveChanged(object sender, EventArgs e)
        {
            if (!(e is DataEventArgs<bool> e2)) return;

            if (e2.Value)
            {
                // アクティブ化の処理
                foreach (var (item, index) in GetRegionViews().WithIndex())
                {
                    if (item.DataContext is ImagePanelViewModel vm)
                        vm.UpdateImageSource(index);
                }
            }
            else
            {
                // 非アクティブ化の処理
                MainImages.AdaptImageListTracks();
            }
        }

        private void SwapImageViewModels()
        {
            var views = GetRegionViews();
            var viewsLength = views.Count();
            //if (viewsLength < 2) return;

            // 内回り
            var head = views.First().DataContext;
            for (int i = 0; i < viewsLength - 1; i++)
            {
                views.ElementAt(i).DataContext = views.ElementAt(i + 1).DataContext;
            }
            views.Last().DataContext = head;

            MainImages.RotateInnerTrack();
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (SetProperty(ref _isActive, value))
                {
                    SwapImagesCommand.IsActive = value;
                    IsActiveChanged?.Invoke(this, new DataEventArgs<bool>(value));
                }
            }
        }

        public event EventHandler IsActiveChanged;

    }
}
